using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace EE.PathfindingSystem {
    internal enum Heuristics {
        VectorMagnitude,
        Manhattan,
        Euclidean
    }
    internal static class HeuristicsUtil {
        public static int GetDistance(Heuristics heuristicMethod, Node nodeA, Node nodeB) {

            switch (heuristicMethod) {
                case Heuristics.VectorMagnitude: return GetVectorMagnitudeDistance(nodeA, nodeB);
                case Heuristics.Manhattan: return GetManhattanDistance(nodeA, nodeB);
                case Heuristics.Euclidean: return GetEuclideanDistance(nodeA, nodeB);
                default:
                    return GetManhattanDistance(nodeA, nodeB);
            }
        }

        /// <summary>
        /// Gets heuristic distance from nodeA to nodeB using Manhattan distance
        /// </summary>
        private static int GetManhattanDistance(Node nodeA, Node nodeB) {
            int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }

        /// <summary>
        /// Gets heuristic distance from nodeA to nodeB using basic distance
        /// </summary>
        private static int GetVectorMagnitudeDistance(Node nodeA, Node nodeB) {
            int distance = (int)((nodeA.WorldPosition - nodeB.WorldPosition).magnitude);
            return distance;
        }

        /// <summary>
        /// Gets heuristic distance from nodeA to nodeB using Euclidean distance
        /// </summary>
        private static int GetEuclideanDistance(Node nodeA, Node nodeB) {
            int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

            return 10 * (int)Mathf.Sqrt(dstX * dstX + dstY * dstY);
        }
    }
    internal static class AStar {

        /// <summary>
        /// Creates path from startPos to targetPos using A*.
        /// </summary>
        public static Vector2[] FindPath(Node startNode, Node goalNode, int maxsize, Heuristics heuristicMethod = Heuristics.Manhattan, float heuristicMultiplier = 1) {

            var openSet = new Heap<Node>(maxsize);
            var closedSet = new Heap<Node>(maxsize);

            if (!goalNode.Walkable || !startNode.Walkable) {
#if UNITY_EDITOR
                UnityEngine.Debug.Log("Start or goal inside collider.");
#endif
                return new Vector2[0];
            }


            openSet.Add(startNode);

            Node currentNode;

            while (openSet.Count > 0) {
                currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == goalNode) {
                    return RetracePath(startNode, goalNode);
                }
                foreach (Node neighbour in currentNode.Neighbours) {
                    if (neighbour == null || !neighbour.Walkable || closedSet.Contains(neighbour)) {
                        continue;
                    }

                    int newCostToNeighbour = currentNode.GCost + HeuristicsUtil.GetDistance(heuristicMethod, currentNode, neighbour) + neighbour.MovementPenalty;
                    if (newCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour)) {
                        var heuristicCost = HeuristicsUtil.GetDistance(heuristicMethod, neighbour, goalNode) * heuristicMultiplier;

                        neighbour.UpdaterHeuristics(newCostToNeighbour, currentNode, Mathf.RoundToInt(heuristicCost));

                        if (openSet.Contains(neighbour)) {
                            openSet.UpdateItem(neighbour);
                        }
                        else {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
#if UNITY_EDITOR
            UnityEngine.Debug.Log("Path not found!");
#endif
            return new Vector2[0];
        }

        private static Vector2[] RetracePath(Node startNode, Node targetNode) {
            List<Vector2> path = new List<Vector2>();
            Node currentNode = targetNode;

            while (currentNode != startNode) {
                path.Add(currentNode.WorldPosition);
                currentNode = currentNode.Parent;
            }

            Vector2[] waypoints = new Vector2[path.Count];

            int pathLength = 0;
            for (int i = path.Count - 1; i >= 0; i--) {
                waypoints[pathLength] = path[i];
                pathLength++;
            }
            return waypoints;
        }


    }

}