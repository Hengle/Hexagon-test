using System.Collections.Generic;
using Hexagon.ECS.Others;
using Hexagon.Services.Map;
using Svelto.ECS;
using UnityEngine;

namespace Hexagon.ECS.Map
{
    public class MapGeneratorEngine : IEngine
    {
        //private readonly IEntityFactory _entityFactory;
        //private readonly PrefabsDictionary _prefabsDictionary;
        private readonly HexGrid _mapManager;

        public MapGeneratorEngine(
            HexGrid mapManager, IEntityFactory entityFactory, PrefabsDictionary prefabsDictionary)
        {
            _mapManager = mapManager;
            //_entityFactory = entityFactory;
            //_prefabsDictionary = prefabsDictionary;

            GenerateMap();
        }

        private void GenerateMap()
        {
           _mapManager.Prepare();
   
        }
        
    }
}
