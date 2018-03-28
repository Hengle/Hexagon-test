using System.Collections;
using System.Collections.Generic;
using Hexagon.ECS.Ability;
using Hexagon.ECS.Battle;
using Hexagon.ECS.Others;
using Hexagon.Services.UI;
using Svelto.ECS;
using UnityEngine;

namespace Hexagon.ECS.Unit {
    public class BattleHudEngine : IQueryingEntityViewEngine, IStep<TurnInfo> {


        public BattleHudEngine(UIService uiService) {
            _uiService = uiService;
        }

        public IEntityViewsDB entityViewsDB { private get; set; }

        public void Ready() {

        }

        public void Step(ref TurnInfo token, int condition) {
            if (condition == TurnCondition.starting)
            {
                if (entityViewsDB.TryQueryEntityView(token.entityID, out _currentUnitEntityView)) {
                    UpdateHud(_currentUnitEntityView);
                    _currentUnitEntityView.turnComponent.moveCounter.NotifyOnValueSet(Moved);
                    AbilitySlot.SlotClickedEvent += OnAbilitySelected;
                }
            }
            else
            {
                _currentUnitEntityView.turnComponent.moveCounter.StopNotify(Moved);
                AbilitySlot.SlotClickedEvent -= OnAbilitySelected;
                _currentUnitEntityView = null;
            }
            
        }

        void OnAbilitySelected(int abilityID)
        {
            _currentUnitEntityView.inputComponent.abilitySelected.value = abilityID;
        }

        void UpdateHud(UnitEntityView entityView)
        {
            UpdatePoints(entityView);
            UpdateBar(entityView);
            UpdateAbilityBar(entityView);
        }

        void UpdateAbilityBar(UnitEntityView entityView)
        {
            var abilities = entityViewsDB.QueryGroupedEntityViews<AbilityEntityView>(entityView.ID);

            for (int i = 0; i < _uiService.ActionBar.AbilitySlots.Count; i++)
            {
                if (i < abilities.Count)
                {
                    _uiService.ActionBar.AbilitySlots[i].Icon = abilities[i].abilityComponent.icon;
                    _uiService.ActionBar.AbilitySlots[i].Activated = true;
                    _uiService.ActionBar.AbilitySlots[i].abilityID = abilities[i].ID;
                }
                else
                {
                    _uiService.ActionBar.AbilitySlots[i].Activated = false;
                }
                

            }
        }

        void UpdatePoints(UnitEntityView entityView)
        {
            int movePoints = entityView.movementComponent.maxMove - entityView.turnComponent.moveCounter.value;
            _uiService.ActionBar.movePoints = movePoints;
        }

        void UpdateBar(UnitEntityView entityView)
        {
            _uiService.ActionBar.currentHealth = entityView.healthComponent.currentHealth.value;
            _uiService.ActionBar.maxHealth = entityView.healthComponent.maxHealth.value;
        }

        void Moved(int id, int value)
        {
            UnitEntityView entityView;
            if (entityViewsDB.TryQueryEntityView(id, out entityView))
            {
                UpdatePoints(entityView);
            }            
        }

        private readonly UIService _uiService;
        private UnitEntityView _currentUnitEntityView;
    }

}
