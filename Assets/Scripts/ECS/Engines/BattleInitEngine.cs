using System.Collections;
using System.Collections.Generic;
using Hexagon.ECS.Others;
using Hexagon.ECS.Unit;
using Svelto.ECS;
using Svelto.Factories;
using UnityEngine;

namespace Hexagon.ECS.Battle
{
    public class BattleInitEngine : SingleEntityViewEngine<BattleEntityView>, IQueryingEntityViewEngine
    {
        private readonly IEntityFactory _entityFactory;
        private readonly IEntityFunctions _entityFunctions;
        private readonly PrefabsDictionary _prefabsDictionary;

        public BattleInitEngine(IEntityFunctions entityFunctions, PrefabsDictionary prefabsDictionary, IEntityFactory entityFactory)
        {
            _entityFunctions = entityFunctions;
            _prefabsDictionary = prefabsDictionary;
            _entityFactory = entityFactory;

            var go = _prefabsDictionary.Istantiate("BattleEntity");
            var implementors = new List<IImplementor> {
                new BattleImplementor()
            };
            _entityFactory.BuildEntity<BattleEntityDescriptor>(
                go.GetInstanceID(), implementors.ToArray());
        }

        IEnumerator pedra(BattleEntityView entityView)
        {
            yield return new WaitForSeconds(2f);

            var unitsInMap = entityViewsDB.QueryEntityViews<UnitEntityView>();
            foreach (var unitEntityView in unitsInMap) {
                _entityFunctions.SwapEntityGroup(unitEntityView.ID, 1, entityView.ID);
                unitEntityView.turnComponent.isInBattle = true;
            }

            entityView.battleStateComponent.state.value = BattleState.Starting;
        }

        protected override void Add(BattleEntityView entityView)
        {
            pedra(entityView).Run();
        }

        protected override void Remove(BattleEntityView entityView)
        {

        }

        public IEntityViewsDB entityViewsDB { private get; set; }
        public void Ready()
        {

        }


    }
}