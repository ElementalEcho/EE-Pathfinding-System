using NUnit.Framework;
using UnityEngine;

namespace EE.PathfindingSystem.EditModeTests {
    public class AstarTests {
        [Test]
        public void AStar_FindPath() {
            var grid = new TestGrid(Vector2.zero, new Vector2(3, 3), 0.5f, 10, 1, new TerrainType[0], Connections.directional8, 1);
            var colliderString = "000000000";
            grid.CreateGrid(colliderString);


            var path = AStar.FindPath(grid.GetNode(0,0), grid.GetNode(2, 2), grid.Maxsize);

            Assert.AreEqual(2, path.Length);
            Assert.AreEqual(grid.GetNode(1, 1).WorldPosition, path[0]);
            Assert.AreEqual(grid.GetNode(2, 2).WorldPosition, path[1]);
        }
        [Test]
        public void AStar_FindPath_Collider() {
            var grid = new TestGrid(Vector2.zero, new Vector2(3, 3), 0.5f, 10, 1, new TerrainType[0], Connections.directional8, 1);
            var colliderString = "000010000";
            grid.CreateGrid(colliderString);


            var path = AStar.FindPath(grid.GetNode(0, 0), grid.GetNode(2, 2), grid.Maxsize);

            Assert.AreEqual(3, path.Length);
            Assert.AreEqual(grid.GetNode(0, 1).WorldPosition, path[0]);
            Assert.AreEqual(grid.GetNode(1, 2).WorldPosition, path[1]);
            Assert.AreEqual(grid.GetNode(2, 2).WorldPosition, path[2]);

        }
        [Test]
        public void AStar_FindPath_ColliderDontcutcorners() {
            var grid = new TestGrid(Vector2.zero, new Vector2(3, 3), 0.5f, 10, 1, new TerrainType[0], Connections.directional8DontCutCorners, 1);
            var colliderString = "000010000";
            grid.CreateGrid(colliderString);


            var path = AStar.FindPath(grid.GetNode(0, 0), grid.GetNode(2, 2), grid.Maxsize);

            Assert.AreEqual(4, path.Length);
            Assert.AreEqual(grid.GetNode(0, 1).WorldPosition, path[0]);
            Assert.AreEqual(grid.GetNode(0, 2).WorldPosition, path[1]);
            Assert.AreEqual(grid.GetNode(1, 2).WorldPosition, path[2]);
            Assert.AreEqual(grid.GetNode(2, 2).WorldPosition, path[3]);
        }
        [Test]
        public void AStar_FindPath_ColliderDontcutcorners2() {
            var grid = new TestGrid(Vector2.zero, new Vector2(3, 3), 0.5f, 10, 1, new TerrainType[0], Connections.directional8DontCutCorners, 1);
            var colliderString = "000011000";
            grid.CreateGrid(colliderString);


            var path = AStar.FindPath(grid.GetNode(0, 0), grid.GetNode(2, 2), grid.Maxsize);

            Assert.AreEqual(4, path.Length);
            Assert.AreEqual(grid.GetNode(1, 0).WorldPosition, path[0]);
            Assert.AreEqual(grid.GetNode(2, 0).WorldPosition, path[1]);
            Assert.AreEqual(grid.GetNode(2, 1).WorldPosition, path[2]);
            Assert.AreEqual(grid.GetNode(2, 2).WorldPosition, path[3]);
        }
        [Test]
        public void AStar_FindPath_TargetInsideCollider() {
            var grid = new TestGrid(Vector2.zero, new Vector2(3, 3), 0.5f, 10, 1, new TerrainType[0], Connections.directional8, 1);
            var colliderString = "000000001";
            grid.CreateGrid(colliderString);

            var path = AStar.FindPath(grid.GetNode(0, 0), grid.GetNode(2, 2), grid.Maxsize);

            Assert.AreEqual(0, path.Length);
        }
        [Test]
        public void AStar_FindPath_StartInsideCollider() {
            var grid = new TestGrid(Vector2.zero, new Vector2(3, 3), 0.5f, 10, 1, new TerrainType[0], Connections.directional8, 1);
            var colliderString = "100000000";
            grid.CreateGrid(colliderString);

            var path = AStar.FindPath(grid.GetNode(0, 0), grid.GetNode(2, 2), grid.Maxsize);

            Assert.AreEqual(0, path.Length);
        }
        [Test]
        public void AStar_FindPath_NoPathAvaileble() {
            var grid = new TestGrid(Vector2.zero, new Vector2(3, 3), 0.5f, 10, 1, new TerrainType[0], Connections.directional8, 1);
            var colliderString = "000111000";
            grid.CreateGrid(colliderString);

            var path = AStar.FindPath(grid.GetNode(0, 0), grid.GetNode(2, 2), grid.Maxsize);

            Assert.AreEqual(0, path.Length);
        }

        [Test]
        public void HeuristicsUtil_GetManhattanDistance() {

            var node1 = new Node(NodeType.walkable, new Vector2(0,0), 0, 0, 0);
            var node2 = new Node(NodeType.walkable, new Vector2(10, 10), 1, 1, 0);

            int distance = HeuristicsUtil.GetDistance(Heuristics.Manhattan, node1, node2);

            Assert.AreEqual(14,distance);
            distance = HeuristicsUtil.GetDistance(Heuristics.Manhattan, node2, node1);

            Assert.AreEqual(14, distance);
        }
        [Test]
        public void HeuristicsUtil_GetVectorMagnitudeDistance() {

            var node1 = new Node(NodeType.walkable, new Vector2(0, 0), 0, 0, 0);
            var node2 = new Node(NodeType.walkable, new Vector2(10, 10), 1, 1, 0);

            int distance = HeuristicsUtil.GetDistance(Heuristics.VectorMagnitude, node1, node2);

            Assert.AreEqual(14, distance);
            distance = HeuristicsUtil.GetDistance(Heuristics.VectorMagnitude, node2, node1);

            Assert.AreEqual(14, distance);

        }
        [Test]
        public void HeuristicsUtil_GetEuclideanDistance() {


            var node1 = new Node(NodeType.walkable, new Vector2(0, 0), 0, 0, 0);
            var node2 = new Node(NodeType.walkable, new Vector2(10, 10), 1, 1, 0);

            int distance = HeuristicsUtil.GetDistance(Heuristics.Euclidean, node1, node2);

            Assert.AreEqual(10, distance);
            distance = HeuristicsUtil.GetDistance(Heuristics.Euclidean, node2, node1);

            Assert.AreEqual(10, distance);
        }
    }
}