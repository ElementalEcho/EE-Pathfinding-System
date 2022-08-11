using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using EE.PathfindingSystem;

namespace EE.PathfindingSystem.EditModeTests {
    internal class TestNode : Node {
        public TestNode(NodeType _walkable, Vector2 _worldPos, int _gridX, int _gridY, int _penalty) : base(_walkable, _worldPos, _gridX, _gridY, _penalty) {}


    }
    public class NodeTests {
        [Test]
        public void Node_IsWalkable_True() {
            var node1 = new Node(NodeType.walkable, new Vector2(0, 0), 0, 0, 0);

            Assert.IsTrue(node1.Walkable);


        }
        [Test]
        public void Node_IsWalkable_False() {
            var node1 = new Node(NodeType.obstacle, new Vector2(0, 0), 0, 0, 0);

            Assert.IsFalse(node1.Walkable);
        }
        [Test]
        public void Node_IsWalkable_GridArea() {
            var node1 = new Node(NodeType.walkable, new Vector2(0, 0), 0, 0, 0);
            node1.SetGridArea(11);
            bool walkable = node1.IsWalkable(11);
            Assert.IsTrue(walkable);

            walkable = node1.IsWalkable(2);
            Assert.IsFalse(walkable);
        }
        [Test]
        public void Node_UpdaterHeuristics() {
            var node = new Node(NodeType.walkable, new Vector2(0, 0), 0, 0, 0);
            var parent = new Node(NodeType.walkable, new Vector2(0, 0), 0, 0, 0);

            node.UpdaterHeuristics(15, parent, 13);
            Assert.AreEqual(15,node.GCost);
            Assert.AreEqual(parent, node.Parent);
            Assert.AreEqual(13, node.HCost);

        }

        [Test]
        public void Node_CompareTo() {
            var node1 = new Node(NodeType.walkable, new Vector2(0, 0), 0, 0, 0);
            var node2 = new Node(NodeType.walkable, new Vector2(0, 0), 0, 0, 0);

            node1.UpdaterHeuristics(100, null, 100);
            node2.UpdaterHeuristics(50, null, 50);

            int comparison = node1.CompareTo(node2);

            Assert.AreEqual(-1, comparison);
        }

        [Test]
        public void Node_CompareTo_reverse() {
            var node1 = new Node(NodeType.walkable, new Vector2(0, 0), 0, 0, 0);
            var node2 = new Node(NodeType.walkable, new Vector2(0, 0), 0, 0, 0);

            node1.UpdaterHeuristics(50, null, 50);
            node2.UpdaterHeuristics(100, null, 100);

            int comparison = node1.CompareTo(node2);

            Assert.AreEqual(1, comparison);
        }
        [Test]
        public void Node_CompareTo_0f() {
            var node1 = new Node(NodeType.walkable, new Vector2(0, 0), 0, 0, 0);
            var node2 = new Node(NodeType.walkable, new Vector2(0, 0), 0, 0, 0);

            node1.UpdaterHeuristics(-50, null, 50);
            node2.UpdaterHeuristics(-100, null, 100);

            int comparison = node1.CompareTo(node2);

            Assert.AreEqual(1, comparison);
        }
        [Test]
        public void Node_CompareTo_0f_reverse() {
            var node1 = new Node(NodeType.walkable, new Vector2(0, 0), 0, 0, 0);
            var node2 = new Node(NodeType.walkable, new Vector2(0, 0), 0, 0, 0);

            node1.UpdaterHeuristics(-50, null, 50);
            node2.UpdaterHeuristics(-100, null, 100);

            int comparison = node1.CompareTo(node2);

            Assert.AreEqual(1, comparison);
        }
        [Test]
        public void Node_CompareTo_equal() {
            var node1 = new Node(NodeType.walkable, new Vector2(0, 0), 0, 0, 0);
            var node2 = new Node(NodeType.walkable, new Vector2(0, 0), 0, 0, 0);

            node1.UpdaterHeuristics(-25, null, 75);
            node2.UpdaterHeuristics(-50, null, 100);

            int comparison = node1.CompareTo(node2);

            Assert.AreEqual(1, comparison);
        }
        [Test]
        public void Node_CompareTo_0f_equal() {
            var node1 = new Node(NodeType.walkable, new Vector2(0, 0), 0, 0, 0);
            var node2 = new Node(NodeType.walkable, new Vector2(0, 0), 0, 0, 0);

            node1.UpdaterHeuristics(-50, null, 50);
            node2.UpdaterHeuristics(-50, null, 50);

            int comparison = node1.CompareTo(node2);

            Assert.AreEqual(0, comparison);
        }
    }
}