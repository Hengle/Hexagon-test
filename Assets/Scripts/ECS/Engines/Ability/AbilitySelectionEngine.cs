using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DuloGames.UI;
using Hexagon.ECS.Battle;
using Hexagon.ECS.Others;
using Hexagon.ECS.Unit;
using Hexagon.Services.UI;
using Svelto.ECS;
using UnityEngine;

namespace Hexagon.ECS.Ability {
    public class AbilitySelectionEngine : SingleEntityViewEngine<UnitEntityView>, IQueryingEntityViewEngine {

        public AbilitySelectionEngine(UIService uiService, ISequencer abilityUseSequence) {
            _uiService = uiService;
            _abilityUseSequence = abilityUseSequence;
        }

        public IEntityViewsDB entityViewsDB { private get; set; }

        public void Ready() 
        {
        }


        void OnAbilitySelected(int senderId, int abilityID)
        {
            UnitEntityView entityView;
            if (entityViewsDB.TryQueryEntityView(senderId, out entityView))
            {
                var abilities = entityViewsDB.QueryGroupedEntityViews<AbilityEntityView>(senderId);
                var matches = abilities.Where(x => x.ID == abilityID);

                if (matches.Count() != 0) {
                    AbilitySelectInfo abilitySelectInfo = new AbilitySelectInfo(senderId, abilityID, false);
                    entityView.turnComponent.state = UnitTurnState.Ability;
                    entityView.turnComponent.currentAbility = abilityID;
                    _abilityUseSequence.Next(this, ref abilitySelectInfo);
                }
            }          
        }

        void OnCancelInput(int senderId, bool cancel)
        {

            UnitEntityView entityView;
            if (entityViewsDB.TryQueryEntityView(senderId, out entityView)) {
                AbilitySelectInfo abilitySelectInfo = new AbilitySelectInfo(senderId, -1, true);
                entityView.turnComponent.state = UnitTurnState.Movement;
                entityView.turnComponent.currentAbility = -1;
                _abilityUseSequence.Next(this, ref abilitySelectInfo);              
            }
        }

        protected override void Add(UnitEntityView entityView) {
            entityView.inputComponent.abilitySelected.NotifyOnValueSet(OnAbilitySelected);
            entityView.inputComponent.cancel.NotifyOnValueSet(OnCancelInput);
        }

        protected override void Remove(UnitEntityView entityView) {
            entityView.inputComponent.abilitySelected.StopNotify(OnAbilitySelected);
            entityView.inputComponent.cancel.StopNotify(OnCancelInput);
        }

        private readonly UIService _uiService;
        private readonly ISequencer _abilityUseSequence;
    }

}
