using System;

namespace NationsWars.Sim
{
    public struct GridPos : IEquatable<GridPos>
    {
        public readonly int X;
        public readonly int Y;

        public GridPos(int x, int y) { X = x; Y = y; }

        public bool Equals(GridPos other) { return X == other.X && Y == other.Y; }
        public override bool Equals(object obj) { return obj is GridPos && Equals((GridPos)obj); }
        public override int GetHashCode() { return X * 73856093 ^ Y * 19349663; }
        public override string ToString() { return "(" + X + ", " + Y + ")"; }
    }
}
