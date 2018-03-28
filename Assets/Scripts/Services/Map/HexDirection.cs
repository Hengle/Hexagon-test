using Hexagon.Services.Map;
using UnityEngine;

namespace Hexagon
{
    public enum HexDirection
    {
        NE,
        E,
        SE,
        SW,
        W,
        NW
    }

    public static class HexDirectionExtensions
    {

        public static HexDirection GetDirection(this HexCell cell, HexCell other) {
            if (other == null) return 0;
            HexCoordinates a = cell.coordinates;
            HexCoordinates b = other.coordinates;
            HexDirection d;
            if (b.X > a.X) {
                if (b.Z < a.Z) d = HexDirection.SE;
                else d = HexDirection.E;
            }
            else if (b.X < a.X) {
                if (b.Z > a.Z) d = HexDirection.NW;
                else d = HexDirection.W;
            }
            else {
                if (b.Z < a.Z) d = HexDirection.SW;
                else d = HexDirection.NE;
            }
            return d;
        }

        public static HexDirection Opposite(this HexDirection direction)
        {
            return (int) direction < 3 ? direction + 3 : direction - 3;
        }

        public static HexDirection Previous(this HexDirection direction)
        {
            return direction == HexDirection.NE ? HexDirection.NW : direction - 1;
        }

        public static HexDirection Next(this HexDirection direction)
        {
            return direction == HexDirection.NW ? HexDirection.NE : direction + 1;
        }

        public static HexDirection Previous2(this HexDirection direction)
        {
            direction -= 2;
            return direction >= HexDirection.NE ? direction : direction + 6;
        }

        public static HexDirection Next2(this HexDirection direction)
        {
            direction += 2;
            return direction <= HexDirection.NW ? direction : direction - 6;
        }

        public static Vector3 ToEuler(this HexDirection d) {
            return new Vector3(0, (int)d * 60 + 30, 0);
        }

    }
}