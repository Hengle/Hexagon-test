
using System.Collections;
using System.Collections.Generic;
using Hexagon.ECS.Player;
using Hexagon.ECS.Unit;
using Hexagon.Services.Map;
using Svelto.ECS;
using Svelto.Tasks;
using UnityEngine;

namespace Hexagon.ECS.Engines.Unit
{
    public class UnitMovementEngine : SingleEntityViewEngine<PlayerEntityView>, IQueryingEntityViewEngine
    {
        private readonly HexGrid _mapManager;


        public UnitMovementEngine(HexGrid mapManager)
        {
            _mapManager = mapManager;
        }

        public void Ready()
        {
        }

        public IEntityViewsDB entityViewsDB { set; private get; }

        protected override void Add(PlayerEntityView entityView)
        {
            entityView.inputComponent.cellClick.NotifyOnValueSet(MoveToTarget);
            Place(entityView);
        }

        protected override void Remove(PlayerEntityView entityView)
        {
        }

        private void MoveToTarget(int id, HexCoordinates target)
        {
            PlayerEntityView entityView;
            if (entityViewsDB.TryQueryEntityView(id, out entityView))
            {
                if (entityView.turnComponent.state.value != UnitTurnState.Movement) return;
                int range = entityView.movementComponent.maxMove - entityView.turnComponent.moveCounter.value;
                if (range <= 0) return;
                if (entityView.movementComponent.isWalking) return;
                if (_mapManager.GetCell(target).IsUnderwater) return;
                HexCell fromCell = _mapManager.GetCell(entityView.coordinatesComponent.coordinates);
                HexCell toCell = _mapManager.GetCell(target);
                List<HexCell> path;
                

                if (_mapManager.FindPath(fromCell, toCell, range, out path))
                {
                    entityView.coordinatesComponent.coordinates = target;
                    Traverse(entityView, path);
                    entityView.turnComponent.moveCounter.value += path.Count - 1;
                }


            }
        }

        private void Traverse(PlayerEntityView entityView, List<HexCell> path)
        {
            var coordComp = entityView.coordinatesComponent;
            var moveComp = entityView.movementComponent;

            //if (moveComp.isWalking) return;

            var serialTasks = new SerialTaskCollection();

            for (var i = 1; i < path.Count; i++)
            {
                HexCell from = path[i - 1];
                HexCell to = path[i];
                
                HexDirection dir = from.GetDirection(to);

                if (coordComp.direction != dir)
                {
                    coordComp.direction = dir;
                    serialTasks.Add(Turn(entityView, dir));
                }

                serialTasks.Add(Walk(entityView, to));
            }

            serialTasks.onComplete += () => { moveComp.isWalking = false; };
            serialTasks.Run();
        }

        private IEnumerator Walk(PlayerEntityView entityView, HexCell cell)
        {
            var moveComp = entityView.movementComponent;
            moveComp.isWalking = true;
            var id = moveComp.move(cell.Position);

            while (LeanTween.isTweening(id)) yield return null;
        }

        private IEnumerator Turn(PlayerEntityView entityView, HexDirection dir)
        {
            var moveComp = entityView.movementComponent;
            moveComp.isWalking = false;
            var id = moveComp.rotate(dir.ToEuler());

            while (LeanTween.isTweening(id)) yield return null;
        } 

        private void Place(PlayerEntityView entityView)
        {
            var comp = entityView.coordinatesComponent;
            HexCell cell = _mapManager.GetCell(comp.coordinates);

            entityView.transformComponent.position = cell.Position;
            entityView.transformComponent.rotation = comp.direction.ToEuler();
        }
    }
}
