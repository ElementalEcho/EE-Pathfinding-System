using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using EE.PathfindingSystem;

namespace EE.PathfindingSystem.EditModeTests {
    internal class TestGrid : Grid {
        public TestGrid(Vector2 gridPosition, Vector2 gridWorldSize, float nodeRadius, float nearestNodeDistance, float collisionRadius, TerrainType[] walkableRegions, Connections options, LayerMask unwalkableMask) : base(gridPosition, gridWorldSize, nodeRadius, nearestNodeDistance, collisionRadius, walkableRegions, options, unwalkableMask) {

        }
        public int NumberOfGridAreas => numberOfGridAreas;
        public WalkableTerrain WalkableTerrain => _walkableTerrain;

        public Node[,] ExposeGrid => grid;

    }

    public class GridTests {

        [Test]
        public void GridIsGeneratedCorrectly() {
            TerrainType[] walkableRegions = new TerrainType[] {
                new TerrainType() {
                    terrainMask = 1,
                    terrainPenalty = 100
                }
            };
            var grid = new TestGrid(Vector2.zero, new Vector2(10, 10), 1, 10, 1, walkableRegions, Connections.directional8DontCutCorners, 1);
            var noColliderString = "0000000000000000000000000";
            grid.CreateGrid(noColliderString);

            Assert.AreEqual(5, grid.GridSizeX);
            Assert.AreEqual(5, grid.GridSizeY);
            Assert.AreEqual(2, grid.NumberOfGridAreas);
            Assert.AreEqual(1, (int)grid.WalkableTerrain.walkableMask);

        }
        [Test]
        public void GridShouldNotHaveColliders() {
            var grid = new TestGrid(Vector2.zero, new Vector2(10, 10), 1, 10, 1, new TerrainType[0], Connections.directional8DontCutCorners, 1);
            var noColliderString = "0000000000000000000000000";
            grid.CreateGrid(noColliderString);

            var nodes = grid.ExposeGrid;
            for (int x = 0; x < grid.GridSizeX; x++) {
                for (int y = 0; y < grid.GridSizeY; y++) {
                    Assert.AreEqual(true, nodes[x,y].Walkable);
                    Assert.AreEqual(0, nodes[x, y].MovementPenalty);
                    Vector2 worldPoint = new Vector2(x * 2 - 4, y * 2 - 4);

                    Assert.AreEqual(worldPoint, nodes[x, y].WorldPosition);

                }
            }
        }
        [Test]
        public void GridShouldbeFullOfColliders() {
            var grid = new TestGrid(Vector2.zero, new Vector2(10, 10), 1, 10, 1, new TerrainType[0], Connections.directional8DontCutCorners, 1);
            var colliderString = "1111111111111111111111111";
            grid.CreateGrid(colliderString);

            var nodes = grid.ExposeGrid;
            for (int x = 0; x < grid.GridSizeX; x++) {
                for (int y = 0; y < grid.GridSizeY; y++) {
                    Assert.AreEqual(false, nodes[x, y].Walkable);
                    Assert.AreEqual(0, nodes[x, y].MovementPenalty);
                    Vector2 worldPoint = new Vector2(x * 2 - 4, y * 2 - 4);

                    Assert.AreEqual(worldPoint, nodes[x, y].WorldPosition);

                }
            }
        }
        [Test]
        public void NodesShouldHaveNeighbours_directional8() {
            var grid = new TestGrid(Vector2.zero, new Vector2(3, 3), 0.5f, 10, 1, new TerrainType[0], Connections.directional8, 1);
            var colliderString = "000000000";
            grid.CreateGrid(colliderString);

            var node = grid.GetNode(1, 1);
            Assert.IsNotNull(node);

            NodeAssert(grid.GetNode(0, 0), node.Neighbours[0]);
            NodeAssert(grid.GetNode(0, 1), node.Neighbours[1]);
            NodeAssert(grid.GetNode(0, 2), node.Neighbours[2]);
            NodeAssert(grid.GetNode(1, 0), node.Neighbours[3]);
            NodeAssert(grid.GetNode(1, 2), node.Neighbours[4]);
            NodeAssert(grid.GetNode(2, 0), node.Neighbours[5]);
            NodeAssert(grid.GetNode(2, 1), node.Neighbours[6]);
            NodeAssert(grid.GetNode(2, 2), node.Neighbours[7]);
        }
        [Test]
        public void NodesShouldHaveNeighbours_directional4() {
            var grid = new TestGrid(Vector2.zero, new Vector2(3, 3), 0.5f, 10, 1, new TerrainType[0], Connections.directional4, 1);
            var colliderString = "000000000";
            grid.CreateGrid(colliderString);

            var node = grid.GetNode(1, 1);
            Assert.IsNotNull(node);

            NodeAssert(grid.GetNode(0, 1), node.Neighbours[0]);
            NodeAssert(grid.GetNode(1, 0), node.Neighbours[1]);
            NodeAssert(grid.GetNode(1, 2), node.Neighbours[2]);
            NodeAssert(grid.GetNode(2, 1), node.Neighbours[3]);
            Assert.IsNull(node.Neighbours[4]);
            Assert.IsNull(node.Neighbours[5]);
            Assert.IsNull(node.Neighbours[6]);
            Assert.IsNull(node.Neighbours[7]);

        }
        [Test]
        public void NodesShouldHaveNeighbours_directional8DontCutCorners() {
            var grid = new TestGrid(Vector2.zero, new Vector2(3, 3), 0.5f, 10, 1, new TerrainType[0], Connections.directional8DontCutCorners, 1);
            var colliderString = "100000000";
            grid.CreateGrid(colliderString);

            var node = grid.GetNode(1, 1);
            Assert.IsNotNull(node);

            NodeAssert(grid.GetNode(0, 1), node.Neighbours[0]);
            NodeAssert(grid.GetNode(0, 2), node.Neighbours[1]);
            NodeAssert(grid.GetNode(1, 0), node.Neighbours[2]);
            NodeAssert(grid.GetNode(1, 2), node.Neighbours[3]);
            NodeAssert(grid.GetNode(2, 0), node.Neighbours[4]);
            NodeAssert(grid.GetNode(2, 1), node.Neighbours[5]);
            NodeAssert(grid.GetNode(2, 2), node.Neighbours[6]);
        }

        [Test]
        public void Nodes_ShouldHaveDifferentAreas() {
            var grid = new TestGrid(Vector2.zero, new Vector2(3, 3), 0.5f, 10, 1, new TerrainType[0], Connections.directional8DontCutCorners, 1);
            var colliderString = "011111110";
            grid.CreateGrid(colliderString);

            Assert.AreEqual(1, grid.GetNode(0, 0).GridAreaID);
            Assert.AreEqual(0, grid.GetNode(0, 1).GridAreaID);
            Assert.AreEqual(0, grid.GetNode(0, 2).GridAreaID);
            Assert.AreEqual(0, grid.GetNode(1, 0).GridAreaID);
            Assert.AreEqual(0, grid.GetNode(1, 1).GridAreaID);
            Assert.AreEqual(0, grid.GetNode(1, 2).GridAreaID);
            Assert.AreEqual(0, grid.GetNode(2, 0).GridAreaID);
            Assert.AreEqual(0, grid.GetNode(2, 1).GridAreaID);
            Assert.AreEqual(2, grid.GetNode(2, 2).GridAreaID);
        }

        private void NodeAssert(Node node1, Node node2) {
            Assert.AreEqual(node1, node2, "Nodes are not same. Grid Node X:" + node1.GridX + " Node X: " + node2.GridX + " Y:" + node1.GridY + " Node Y: " + node2.GridY);

        }
        [Test]
        public void AddWalkableRegionsToDictonaryTest() {
            TerrainType[] walkableRegions = new TerrainType[] {
                new TerrainType() { 
                    terrainMask = 1,
                    terrainPenalty = 100
                }
            };

            var walkableTerrain = WalkableTerrain.CreateWalkableTerrain(walkableRegions);

            Assert.AreEqual(1 , (int)walkableTerrain.walkableMask);
        }
    }

}
