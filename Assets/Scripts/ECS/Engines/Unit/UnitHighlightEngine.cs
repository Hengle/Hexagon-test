
using System.Collections;
using System.Collections.Generic;
using Hexagon.ECS.Ability;
using Hexagon.Services.Map;
using Svelto.ECS;
using Svelto.Tasks;
using UnityEngine;

namespace Hexagon.ECS.Unit {
    public class UnitHighlightEngine : SingleEntityViewEngine<UnitEntityView>, IQueryingEntityViewEngine {
        private readonly HexGrid _mapManager;


        public UnitHighlightEngine(HexGrid mapManager) {
            _mapManager = mapManager;
        }

        public void Ready() {
        }

        public IEntityViewsDB entityViewsDB { set; private get; }

        protected override void Add(UnitEntityView entityView) {
            entityView.inputComponent.selection.NotifyOnValueSet(OnCellSelection);
            entityView.turnComponent.state.NotifyOnValueSet(OnStateChange);
        }

        protected override void Remove(UnitEntityView entityView) {
            entityView.inputComponent.selection.StopNotify(OnCellSelection);
            entityView.turnComponent.state.StopNotify(OnStateChange);
        }

        private void OnCellSelection(int id, HexCoordinates target) {
            UnitEntityView entityView;
            if (entityViewsDB.TryQueryEntityView(id, out entityView))
            {
                switch (entityView.turnComponent.state.value)
                {
                    case UnitTurnState.Movement:
                        MovementHighlight(entityView, target);
                        break;

                    case UnitTurnState.AbilitySelected:
                        AbilityHighlight(entityView, target);
                        break;
                }
            }
        }


        private void OnStateChange(int unitId, UnitTurnState state)
        {
            _mapManager.UnHighlightAllCells();
            UnitEntityView entityView;
            if (entityViewsDB.TryQueryEntityView(unitId, out entityView))
            {
                if (entityView.turnComponent.state.value == UnitTurnState.AbilitySelected)
                {
                    AbilityHighlight(entityView, entityView.inputComponent.selection.value);
                }               
            }
        }

        private void MovementHighlight(UnitEntityView entityView, HexCoordinates target)
        {
            int range = entityView.movementComponent.maxMove - entityView.turnComponent.moveCounter.value;
            if (range <= 0) return;
            if (_mapManager.GetCell(target).IsUnderwater) return;
            HexCell fromCell = _mapManager.GetCell(entityView.coordinatesComponent.coordinates);
            HexCell toCell = _mapManager.GetCell(target);
            List<HexCell> path;

            if (_mapManager.FindPath(fromCell, toCell, range, out path)) {
                path.Remove(fromCell);
                _mapManager.HighlightCells(path, Color.grey);
            }
        }

        private void AbilityHighlight(UnitEntityView entityView, HexCoordinates target)
        {
            _mapManager.UnHighlightAllCells();
            AbilityEntityView abilityView;
            if (entityViewsDB.TryQueryEntityView(entityView.abilityComponent.currentAbility, out abilityView))
            {
                int range = abilityView.rangeComponent.maxRange;
                int area = abilityView.areaComponent.area;
                HexCell fromCell = _mapManager.GetCell(entityView.coordinatesComponent.coordinates);

                List<HexCell> rangeCells = _mapManager.SearchInRange(fromCell, range, false);
                _mapManager.HighlightCells(rangeCells, Color.blue);

                HexCell currentCell = _mapManager.GetCell(target);
                if (currentCell != null && rangeCells.Contains(currentCell))
                {
                    List<HexCell> areaCells = _mapManager.SearchInRange(currentCell, area, true);
                    _mapManager.HighlightCells(areaCells, Color.red);
                }

            }
        }
    }
}
