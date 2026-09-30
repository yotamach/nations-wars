using System.Collections.Generic;
using NationsWars.Sim;
using NUnit.Framework;

namespace NationsWars.Tests
{
    public class AStarTests
    {
        [Test]
        public void StraightLineOnOpenGrid()
        {
            var grid = new NavGrid(10, 10);
            var path = AStar.FindPath(grid, new GridPos(0, 0), new GridPos(9, 0));
            Assert.AreEqual(9, path.Count);
            Assert.AreEqual(new GridPos(9, 0), path[path.Count - 1]);
        }

        [Test]
        public void StartEqualsGoalGivesEmptyPath()
        {
            var path = AStar.FindPath(new NavGrid(4, 4), new GridPos(2, 2), new GridPos(2, 2));
            Assert.IsNotNull(path);
            Assert.AreEqual(0, path.Count);
        }

        [Test]
        public void WalksAroundAWallThroughTheGap()
        {
            var grid = new NavGrid(10, 10);
            grid.BlockRect(5, 0, 1, 9); // wall with a gap at y = 9
            var path = AStar.FindPath(grid, new GridPos(0, 0), new GridPos(9, 0));
            Assert.IsNotNull(path);
            Assert.IsTrue(path.Contains(new GridPos(5, 9)));
            AssertValidPath(grid, new GridPos(0, 0), path);
        }

        [Test]
        public void FullyWalledOffGoalIsUnreachable()
        {
            var grid = new NavGrid(10, 10);
            grid.BlockRect(5, 0, 1, 10);
            Assert.IsNull(AStar.FindPath(grid, new GridPos(0, 0), new GridPos(9, 0)));
        }

        [Test]
        public void BlockedGoalIsMovedToTheNearestFreeCell()
        {
            var grid = new NavGrid(10, 10);
            grid.BlockRect(4, 4, 2, 2);
            var path = AStar.FindPath(grid, new GridPos(0, 0), new GridPos(4, 4));
            Assert.IsNotNull(path);
            Assert.IsFalse(grid.IsBlocked(path[path.Count - 1]));
        }

        [Test]
        public void DiagonalStepsDoNotCutCorners()
        {
            var grid = new NavGrid(3, 3);
            grid.SetBlocked(new GridPos(1, 0), true);
            grid.SetBlocked(new GridPos(0, 1), true);
            // (0,0) is sealed in by its two neighbors; the diagonal to (1,1) would cut both corners.
            Assert.IsNull(AStar.FindPath(grid, new GridPos(0, 0), new GridPos(1, 1)));
        }

        [Test]
        public void OutOfBoundsEndpointsGiveNoPath()
        {
            var grid = new NavGrid(4, 4);
            Assert.IsNull(AStar.FindPath(grid, new GridPos(-1, 0), new GridPos(2, 2)));
            Assert.IsNull(AStar.FindPath(grid, new GridPos(0, 0), new GridPos(9, 9)));
        }

        [Test]
        public void SameInputAlwaysGivesTheSamePath()
        {
            var grid = new NavGrid(20, 20);
            grid.BlockRect(8, 2, 2, 14);
            var a = AStar.FindPath(grid, new GridPos(1, 1), new GridPos(18, 17));
            var b = AStar.FindPath(grid, new GridPos(1, 1), new GridPos(18, 17));
            CollectionAssert.AreEqual(a, b);
        }

        static void AssertValidPath(NavGrid grid, GridPos start, List<GridPos> path)
        {
            var prev = start;
            foreach (var p in path)
            {
                Assert.IsFalse(grid.IsBlocked(p));
                Assert.IsTrue(System.Math.Abs(p.X - prev.X) <= 1 && System.Math.Abs(p.Y - prev.Y) <= 1);
                prev = p;
            }
        }
    }
}
