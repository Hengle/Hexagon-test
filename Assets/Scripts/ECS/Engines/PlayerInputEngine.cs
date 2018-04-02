using System.Collections;
using Hexagon.ECS.Battle;
using Hexagon.Services.Map;
using Svelto.ECS;
using Svelto.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hexagon.ECS.Player
{

    public class PlayerInputEngine : IQueryingEntityViewEngine, IStep<TurnInfo>
    {
        

        private readonly ITaskRoutine _taskRoutine;

        public PlayerInputEngine(HexGrid mapManager)
        {
            _mapManager = mapManager;
            _taskRoutine = TaskRunner.Instance.AllocateNewTaskRoutine().SetEnumeratorProvider(ReadInput);
        }

        private IEnumerator ReadInput()
        {
            yield return null;
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _playerEntityView.inputComponent.pass.value = true;                   
                }

                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    _playerEntityView.inputComponent.cancel.value = true;
                }

                RaycastHit hit;
                var ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        if (_mapManager.GetCell(hit.point) != null) {
                            var coord = _mapManager.GetCell(hit.point).coordinates;
                            if (_playerEntityView == null) yield return null;
                            if (!coord.Equals(_playerEntityView.inputComponent.selection.value)) {
                                _playerEntityView.inputComponent.selection.value = coord;
                            }
                            if (Input.GetButtonUp("Fire1")) {
                                _playerEntityView.inputComponent.cellClick.value = coord;
                            }
                        }
                    }
                    
                }
                yield return null;
            }
        }

        public void Step(ref TurnInfo token, int condition)
        {
            if (condition == TurnCondition.starting)
            {
                if (entityViewsDB.TryQueryEntityView(token.entityID, out _playerEntityView)) {
                    _taskRoutine.Start();
                }
            }
            else
            {
                _taskRoutine.Stop();
                _playerEntityView = null;
            }

                  
        }

        public IEntityViewsDB entityViewsDB { private get; set; }
        public void Ready()
        {
        }

        private readonly HexGrid _mapManager;
        private PlayerEntityView _playerEntityView;

    }
}
