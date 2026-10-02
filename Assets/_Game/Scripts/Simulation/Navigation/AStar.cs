using System;
using System.Collections.Generic;

namespace NationsWars.Sim
{
    /// <summary>
    /// 8-direction A* on a NavGrid. Diagonal steps never cut corners. Deterministic:
    /// ties are broken by insertion order, so the same grid always gives the same path.
    /// </summary>
    public static class AStar
    {
        const float Diagonal = 1.41421356f;
        static readonly int[] Dx = { 1, -1, 0, 0, 1, 1, -1, -1 };
        static readonly int[] Dy = { 0, 0, 1, -1, 1, -1, 1, -1 };

        struct Entry
        {
            public float F;
            public int Seq;
            public int Node;
        }

        /// <summary>
        /// Path from start to goal, excluding start and including the final cell. Empty when start equals goal.
        /// Returns null when no path exists. A blocked goal is moved to the nearest free cell.
        /// </summary>
        public static List<GridPos> FindPath(NavGrid grid, GridPos start, GridPos goal)
        {
            if (!grid.InBounds(start) || !grid.InBounds(goal)) return null;
            if (grid.IsBlocked(goal) && !grid.TryFindNearestFree(goal, 8, out goal)) return null;

            var path = new List<GridPos>();
            if (start.Equals(goal)) return path;

            int w = grid.Width;
            int count = w * grid.Height;
            var cost = new float[count];
            var parent = new int[count];
            var closed = new bool[count];
            for (int i = 0; i < count; i++) { cost[i] = float.MaxValue; parent[i] = -1; }

            int startIdx = start.Y * w + start.X;
            int goalIdx = goal.Y * w + goal.X;
            var heap = new List<Entry>();
            int seq = 0;

            cost[startIdx] = 0f;
            Push(heap, new Entry { F = Heuristic(start, goal), Seq = seq++, Node = startIdx });

            while (heap.Count > 0)
            {
                Entry current = Pop(heap);
                if (closed[current.Node]) continue;
                closed[current.Node] = true;
                if (current.Node == goalIdx) break;

                int cx = current.Node % w;
                int cy = current.Node / w;

                for (int d = 0; d < 8; d++)
                {
                    var next = new GridPos(cx + Dx[d], cy + Dy[d]);
                    if (grid.IsBlocked(next)) continue;

                    bool diagonal = d >= 4;
                    if (diagonal &&
                        (grid.IsBlocked(new GridPos(cx + Dx[d], cy)) || grid.IsBlocked(new GridPos(cx, cy + Dy[d]))))
                        continue;

                    int nextIdx = next.Y * w + next.X;
                    if (closed[nextIdx]) continue;

                    float newCost = cost[current.Node] + (diagonal ? Diagonal : 1f);
                    if (newCost >= cost[nextIdx]) continue;

                    cost[nextIdx] = newCost;
                    parent[nextIdx] = current.Node;
                    Push(heap, new Entry { F = newCost + Heuristic(next, goal), Seq = seq++, Node = nextIdx });
                }
            }

            if (parent[goalIdx] < 0) return null;

            for (int node = goalIdx; node != startIdx; node = parent[node])
                path.Add(new GridPos(node % w, node / w));
            path.Reverse();
            return path;
        }

        static float Heuristic(GridPos a, GridPos b)
        {
            int dx = Math.Abs(a.X - b.X);
            int dy = Math.Abs(a.Y - b.Y);
            return (dx + dy) + (Diagonal - 2f) * Math.Min(dx, dy);
        }

        static bool Less(Entry a, Entry b)
        {
            return a.F < b.F || (a.F == b.F && a.Seq < b.Seq);
        }

        static void Push(List<Entry> heap, Entry e)
        {
            heap.Add(e);
            int i = heap.Count - 1;
            while (i > 0)
            {
                int parentIdx = (i - 1) / 2;
                if (!Less(heap[i], heap[parentIdx])) break;
                Entry tmp = heap[i]; heap[i] = heap[parentIdx]; heap[parentIdx] = tmp;
                i = parentIdx;
            }
        }

        static Entry Pop(List<Entry> heap)
        {
            Entry top = heap[0];
            int last = heap.Count - 1;
            heap[0] = heap[last];
            heap.RemoveAt(last);

            int i = 0;
            while (true)
            {
                int l = i * 2 + 1, r = l + 1, smallest = i;
                if (l < heap.Count && Less(heap[l], heap[smallest])) smallest = l;
                if (r < heap.Count && Less(heap[r], heap[smallest])) smallest = r;
                if (smallest == i) break;
                Entry tmp = heap[i]; heap[i] = heap[smallest]; heap[smallest] = tmp;
                i = smallest;
            }
            return top;
        }
    }
}
