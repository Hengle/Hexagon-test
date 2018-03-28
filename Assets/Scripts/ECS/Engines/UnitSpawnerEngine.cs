using System.Collections.Generic;
using System.IO;
using Hexagon.ECS.Ability;
using Hexagon.ECS.Others;
using Hexagon.ECS.Unit;
using Hexagon.Services.Map;
using Svelto.ECS;
using Svelto.Factories;
using UnityEngine;

namespace Hexagon.ECS.Unit
{
    public class UnitSpawnerEngine : IEngine, IQueryingEntityViewEngine
    {
        public UnitSpawnerEngine(PrefabsDictionary prefabsDictionary, IEntityFactory entityFactory, HexGrid mapManager)
        {
            _prefabsDictionary = prefabsDictionary;
            _entityFactory = entityFactory;
            _mapManager = mapManager;

            var cells = _mapManager.GetCells();
                        

            for (int i = 0; i < 2; i++)
            {
                int random;
                do {
                    random = Random.Range(0, cells.Count);
                } while (cells[random].IsUnderwater);

                var coord = cells[random].coordinates;

                var go = _prefabsDictionary.Istantiate("testUnit");
                var implementors = new List<IImplementor>();
                go.GetComponentsInChildren(implementors);
                implementors.Add(new UnitCoordinatesImplementor(coord.X, coord.Z));
                _entityFactory.BuildEntityInGroup<PlayerEntityDescriptor>(
                    go.GetInstanceID(), 1, implementors.ToArray());

                BuildAbilities("FireBolt", go.GetInstanceID());
               if (i == 1) BuildAbilities("IcePunch", go.GetInstanceID());
            }

        }

        void BuildAbilities(string prefabName, int groupId)
        {
            var go = _prefabsDictionary.Istantiate(prefabName);
            var implementors = new List<IImplementor>();
            go.GetComponentsInChildren(implementors);
            _entityFactory.BuildEntityInGroup<AbilityEntityDescriptor>(
                go.GetInstanceID(), groupId, implementors.ToArray());
        }


        public IEntityViewsDB entityViewsDB { set; private get; }

        public void Ready()
        {         
        }

        private readonly IEntityFactory _entityFactory;

        private readonly PrefabsDictionary _prefabsDictionary;
        private readonly HexGrid _mapManager;
    }
}
