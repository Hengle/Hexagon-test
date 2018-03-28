using System.Collections;
using System.Collections.Generic;
using Hexagon.ECS.Battle;
using Svelto.ECS;
using UnityEngine;

namespace Hexagon.ECS.Unit
{
    public class UnitTurnActionEngine : IQueryingEntityViewEngine, IStep<TurnInfo> {
        

        public UnitTurnActionEngine(ISequencer turnSequence)
        {
            _turnSequence = turnSequence;
        }

        public IEntityViewsDB entityViewsDB { private get; set; }

        public void Ready()
        {
            
        }

        public void Step(ref TurnInfo token, int condition) {
            _currentTurn = token;
            var unit = entityViewsDB.QueryEntityView<UnitEntityView>(_currentTurn.entityID);
            unit.turnComponent.isMyTurn.value = true;
            unit.turnComponent.moveCounter.value = 0;
            unit.turnComponent.state = UnitTurnState.Movement;
            unit.turnComponent.currentAbility = -1;
            _turnSequence.Next(this, ref _currentTurn, TurnCondition.starting);
            unit.inputComponent.pass.NotifyOnValueSet(OnPassInput);
        }

        private void OnPassInput(int id, bool pass)
        {
            if (id == _currentTurn.entityID)
            {
                var unit = entityViewsDB.QueryEntityView<UnitEntityView>(id);
                unit.turnComponent.isMyTurn.value = false;
                unit.inputComponent.pass.StopNotify(OnPassInput);
                _turnSequence.Next(this, ref _currentTurn, TurnCondition.ending);              
            }
        }


        private readonly ISequencer _turnSequence;
        private TurnInfo _currentTurn;
    }

}
