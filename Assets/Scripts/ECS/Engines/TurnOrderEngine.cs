using Svelto.ECS;
using Svelto.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Hexagon.ECS.Others;
using Hexagon.ECS.Unit;
using Hexagon.Services.UI;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Hexagon.ECS.Battle
{
    public class TurnOrderEngine : SingleEntityViewEngine<BattleEntityView>, IQueryingEntityViewEngine, IStep<TurnInfo>
    {
        public TurnOrderEngine(ISequencer battleTurnSequence, UIService uiService)
        {
            _taskRoutine = TaskRunner.Instance.AllocateNewTaskRoutine().SetEnumerator(Tick());
            _battleTurnSequence = battleTurnSequence;
            _uiService = uiService;

        }

        public IEntityViewsDB entityViewsDB { set; private get; }

        public void Ready()
        {
        }

        public void Step(ref TurnInfo token, int condition)
        {
            _taskRoutine.Resume();
        }

        protected override void Add(BattleEntityView entityView)
        {
            _battleEntityView = entityView;
            _battleEntityView.battleStateComponent.state.NotifyOnValueSet(stateChanged);
        }

        protected override void Remove(BattleEntityView entityView)
        {
            _battleEntityView = null;
        }

        private void stateChanged(int sender, BattleState state)
        {
            if (state == BattleState.Starting)
            {
                battleStarted();
            }
        }

        private void battleStarted()
        {
            var unitsQuery = entityViewsDB.QueryGroupedEntityViews<UnitEntityView>(_battleEntityView.ID);

            for (int i = 0; i < unitsQuery.Count; i++)
            {

                string name = "Unit " + (i + 1);

                _uiService.TurnList.addUnit(unitsQuery[i].ID, name);
                unitsQuery[i].turnComponent.isMyTurn.NotifyOnValueSet(prep);
            }

            _uiService.TurnList.Active = true;
                _taskRoutine.Start();
        }

        private void prep(int id, bool isMyTurn)
        {
            var matches = _uiService.TurnList.Units.Where(x => x.id == id);
            var turnUnits = matches.ToList();
            if (turnUnits.Count() != 0)
            {
                turnUnits[0].selected = isMyTurn;
            }

        }

        private IEnumerator Tick()
        {
            yield return null;

            while (true)
            {
                var unitsQuery = entityViewsDB.QueryGroupedEntityViews<UnitEntityView>(_battleEntityView.ID);
                List<UnitEntityView> units = unitsQuery.ToList();
                foreach (var unit in unitsQuery)
                {
                    unit.turnComponent.ctrCounter += 1;
                }
                units.Sort((a,b) => a.turnComponent.ctrCounter.CompareTo(b.turnComponent.ctrCounter));
                for (int i = units.Count - 1; i >= 0; i--)
                {
                    if (CanTakeTurn(units[i]))
                    {
                        var turnInfo = new TurnInfo(units[i].ID);
                        _battleTurnSequence.Next(this, ref turnInfo, TurnCondition.starting);
                        _taskRoutine.Pause();
                        yield return null;
                    }
               
                }
            }
        }

        private bool CanTakeTurn(UnitEntityView unitView)
        {
            return unitView.turnComponent.ctrCounter >= 10;
        }

        private readonly ITaskRoutine _taskRoutine;
        private BattleEntityView _battleEntityView;
        private readonly ISequencer _battleTurnSequence;

        private readonly UIService _uiService;
    }
}