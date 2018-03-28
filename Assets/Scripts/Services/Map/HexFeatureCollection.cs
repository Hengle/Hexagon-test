using System;
using UnityEngine;

namespace Hexagon.Services.Map
{
    [Serializable]
    public struct HexFeatureCollection
    {
        public Transform[] prefabs;

        public Transform Pick(float choice)
        {
            return prefabs[(int) (choice * prefabs.Length)];
        }
    }
}