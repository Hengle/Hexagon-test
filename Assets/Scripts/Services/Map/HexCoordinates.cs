using System;
using System.IO;
using UnityEngine;

namespace Hexagon
{
    [Serializable]
    public struct HexCoordinates
    {
        [SerializeField] private int x, z;

        public int X
        {
            get { return x; }
        }

        public int Z
        {
            get { return z; }
        }

        public int Y
        {
            get { return -X - Z; }
        }

        public HexCoordinates(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public int DistanceTo(HexCoordinates other)
        {
            return
                ((x < other.x ? other.x - x : x - other.x) +
                 (Y < other.Y ? other.Y - Y : Y - other.Y) +
                 (z < other.z ? other.z - z : z - other.z)) / 2;
        }

        public static HexCoordinates FromOffsetCoordinates(int x, int z)
        {
            return new HexCoordinates(x - z / 2, z);
        }

        public static HexCoordinates FromPosition(Vector3 position)
        {
            var x = position.x / (HexMetrics.innerRadius * 2f);
            var y = -x;

            var offset = position.z / (HexMetrics.outerRadius * 3f);
            x -= offset;
            y -= offset;

            var iX = Mathf.RoundToInt(x);
            var iY = Mathf.RoundToInt(y);
            var iZ = Mathf.RoundToInt(-x - y);

            if (iX + iY + iZ != 0)
            {
                var dX = Mathf.Abs(x - iX);
                var dY = Mathf.Abs(y - iY);
                var dZ = Mathf.Abs(-x - y - iZ);

                if (dX > dY && dX > dZ)
                    iX = -iY - iZ;
                else if (dZ > dY) iZ = -iX - iY;
            }

            return new HexCoordinates(iX, iZ);
        }

        public override string ToString()
        {
            return "(" +
                   X + ", " + Y + ", " + Z + ")";
        }

        public string ToStringOnSeparateLines()
        {
            return X + "\n" + Y + "\n" + Z;
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(x);
            writer.Write(z);
        }

        public static HexCoordinates Load(BinaryReader reader)
        {
            HexCoordinates c;
            c.x = reader.ReadInt32();
            c.z = reader.ReadInt32();
            return c;
        }
    }
}