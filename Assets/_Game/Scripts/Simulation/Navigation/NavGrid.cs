namespace NationsWars.Sim
{
    public sealed class NavGrid
    {
        public readonly int Width;
        public readonly int Height;
        readonly bool[] blocked;

        public NavGrid(int width, int height)
        {
            Width = width;
            Height = height;
            blocked = new bool[width * height];
        }

        public bool InBounds(GridPos p) { return p.X >= 0 && p.Y >= 0 && p.X < Width && p.Y < Height; }

        public bool IsBlocked(GridPos p) { return !InBounds(p) || blocked[p.Y * Width + p.X]; }

        public void SetBlocked(GridPos p, bool value)
        {
            if (InBounds(p)) blocked[p.Y * Width + p.X] = value;
        }

        public void BlockRect(int x, int y, int w, int h, bool value = true)
        {
            for (int j = y; j < y + h; j++)
                for (int i = x; i < x + w; i++)
                    SetBlocked(new GridPos(i, j), value);
        }

        /// <summary>Closest unblocked cell to origin within maxRadius (ring search, scan order is fixed).</summary>
        public bool TryFindNearestFree(GridPos origin, int maxRadius, out GridPos result)
        {
            if (!IsBlocked(origin)) { result = origin; return true; }

            for (int r = 1; r <= maxRadius; r++)
            {
                bool found = false;
                int bestDist = int.MaxValue;
                GridPos best = origin;
                for (int dy = -r; dy <= r; dy++)
                {
                    for (int dx = -r; dx <= r; dx++)
                    {
                        if (System.Math.Max(System.Math.Abs(dx), System.Math.Abs(dy)) != r) continue;
                        var p = new GridPos(origin.X + dx, origin.Y + dy);
                        if (IsBlocked(p)) continue;
                        int d = dx * dx + dy * dy;
                        if (d < bestDist) { bestDist = d; best = p; found = true; }
                    }
                }
                if (found) { result = best; return true; }
            }

            result = origin;
            return false;
        }
    }
}
