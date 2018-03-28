using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hexagon.ECS.Others
{
    public class PrefabsDictionary
    {
        private readonly Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();

        public PrefabsDictionary(string file)
        {
            var json = File.ReadAllText(file);

            var gameobjects = JsonHelper.getJsonArray<GameObject>(json);

            for (var i = 0; i < gameobjects.Length; i++) prefabs[gameobjects[i].name] = gameobjects[i];
        }

        public GameObject Istantiate(string player)
        {
            return Object.Instantiate(prefabs[player]);
        }
    }
}