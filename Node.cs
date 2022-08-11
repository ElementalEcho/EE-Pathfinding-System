using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EE.PathfindingSystem {

    internal enum NodeType {
        obstacle,
        walkable
    }

    [System.Serializable]
    internal class Node : IHeapItem<Node> {
        private NodeType walkable;
        private Vector2 worldPosition;
        private int gridX;
        private int gridY;
        private int movementPenalty;
        private int gridAreaID = 0;
        private int gCost;
        private int hCost;
        private Node[] neighbours = new Node[8];
        private int heapIndex;

        public int GridX => gridX;
        public int GridY => gridY;
        public int GridAreaID => gridAreaID;
        public Vector2 WorldPosition => worldPosition;
        public int MovementPenalty => movementPenalty;
        public bool Walkable => walkable == NodeType.walkable;
        public int GCost => gCost;
        public int HCost => hCost;
        public int FCost => gCost + hCost;

        private Node parent;
        public Node Parent => parent;
        public Node[] Neighbours => neighbours;

        public int HeapIndex { get => heapIndex; set => heapIndex = value; }

        public void SetGridArea(int value) {
            gridAreaID = value;
        }
        public void SetWalkable(NodeType value) {
            walkable = value;
        }

        public Node(NodeType _walkable, Vector2 _worldPos, int _gridX, int _gridY, int _penalty) {
            walkable = _walkable;
            worldPosition = _worldPos;
            gridX = _gridX;
            gridY = _gridY;
            movementPenalty = _penalty;
        }

        public bool IsWalkable(int nodeArea) {
            return Walkable && (gridAreaID == nodeArea || nodeArea < 0);            
        }

        public void UpdaterHeuristics(int newCostToNeighbour, Node currentNode, int heuristicCost) {
            gCost = newCostToNeighbour;
            hCost = heuristicCost;
            parent = currentNode;
        }

        public int CompareTo(Node nodeToCompare) {
            int compare = nodeToCompare.FCost.CompareTo(FCost);
            return compare == 0 ? nodeToCompare.hCost.CompareTo(hCost)  : compare;
        }

    }

}

