using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DuloGames.UI;
using Hexagon.ECS.Battle;
using Hexagon.ECS.Others;
using Hexagon.ECS.Unit;
using Hexagon.Services.Map;
using Hexagon.Services.UI;
using Svelto.ECS;
using UnityEngine;

namespace Hexagon.ECS.Ability {
    public class AbilitySelectionEngine : SingleEntityViewEngine<UnitEntityView>, IQueryingEntityViewEngine {

        public AbilitySelectionEngine(UIService uiService, ISequencer abilityUseSequence, HexGrid mapManager) {
            _uiService = uiService;
            _abilityUseSequence = abilityUseSequence;
            _mapManager = mapManager;
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
                    AbilitySelectInfo abilitySelectInfo = new AbilitySelectInfo(senderId, abilityID);
                    entityView.abilityComponent.currentAbility = abilityID;
                    entityView.turnComponent.state.value = UnitTurnState.AbilitySelected;
                    _abilityUseSequence.Next(this, ref abilitySelectInfo, AbilityUseCondition.select);
                }
            }          
        }

        void OnCellClick(int senderId, HexCoordinates target)
        {
            UnitEntityView entityView;
            if (entityViewsDB.TryQueryEntityView(senderId, out entityView))
            {
                if (entityView.turnComponent.state.value == UnitTurnState.AbilitySelected)
                {
                    AbilityEntityView abilityView;
                    if (entityViewsDB.TryQueryEntityView(entityView.abilityComponent.currentAbility, out abilityView))
                    {
                        int range = abilityView.rangeComponent.maxRange;
                        int area = abilityView.areaComponent.area;
                        HexCell fromCell = _mapManager.GetCell(entityView.coordinatesComponent.coordinates);

                        List<HexCell> rangeCells = _mapManager.SearchInRange(fromCell, range, false);

                        HexCell currentCell = _mapManager.GetCell(target);
                        if (currentCell != null && rangeCells.Contains(currentCell))
                        {
                            List<HexCell> areaCells = _mapManager.SearchInRange(currentCell, area, true);
                            entityView.abilityComponent.targets = areaCells;
                            entityView.turnComponent.state.value = UnitTurnState.AbilityTargeted;
                        }

                    }
                }
            }
        }

        void OnCancelInput(int senderId, bool cancel)
        {
            UnitEntityView entityView;
            if (entityViewsDB.TryQueryEntityView(senderId, out entityView)) {
                AbilitySelectInfo abilitySelectInfo = new AbilitySelectInfo(senderId, -1);
                if (entityView.turnComponent.state.value == UnitTurnState.AbilitySelected)
                {
                    entityView.turnComponent.state.value = UnitTurnState.Movement;
                    entityView.abilityComponent.currentAbility = -1;
                }

            }
        }

        protected override void Add(UnitEntityView entityView) {
            entityView.inputComponent.abilitySelected.NotifyOnValueSet(OnAbilitySelected);
            entityView.inputComponent.cancel.NotifyOnValueSet(OnCancelInput);
            entityView.inputComponent.cellClick.NotifyOnValueSet(OnCellClick);
        }

        protected override void Remove(UnitEntityView entityView) {
            entityView.inputComponent.abilitySelected.StopNotify(OnAbilitySelected);
            entityView.inputComponent.cancel.StopNotify(OnCancelInput);
            entityView.inputComponent.cellClick.StopNotify(OnCellClick);
        }

        private readonly UIService _uiService;
        private readonly ISequencer _abilityUseSequence;
        private readonly HexGrid _mapManager;
    }

}
