using UnityEngine;
using System.Collections.Generic;

namespace EE.PathfindingSystem {
    public static class PathfindingUtils {
        /// <summary>
        ///Way to reduce number of nodes from path. Only adds nodes that have new direction.This is useful if you have grid based game, but more nodes is better for non grid based game so path looks smooter.
        /// </summary>
        internal static Vector2[] SimplifyPath(List<Node> path) {
            List<Vector2> waypoints = new List<Vector2>();
            Vector2 directionOld = Vector2.zero;

            for (int i = 1; i < path.Count; i++) {
                Vector2 directionNew = new Vector2(path[i - 1].GridX - path[i].GridX, path[i - 1].GridY - path[i].GridY);
                if (directionNew != directionOld) {
                    waypoints.Add(path[i].WorldPosition);
                }
                directionOld = directionNew;
            }
            waypoints.Add(path[path.Count - 1].WorldPosition);
            return waypoints.ToArray();
        }

        /// <summary>
        /// Reduces number of nodes from path. Only adds nodes that are blocked by obstacle. This is useful if you want to have short path, but you can create smoother looking path using dynamic collider check.
        /// </summary>
        public static Vector2[] PathSmooter(Vector2[] path, LayerMask unwalkableMask) {
            List<Vector2> waypoints = new List<Vector2>();
            int currentNode = 0;
            waypoints.Add(path[0]);

            int security = 0;
            for (int i = 1; i < path.Length; i++) {
                security++;
                if (security >= 1000) {
                    Debug.LogError("Crash");
                    break;
                }
                bool cantSeeTarget = Physics2D.Linecast(path[currentNode], path[i], unwalkableMask);
                if (cantSeeTarget) {
                    waypoints.Add(path[i - 1]);
                    currentNode = i - 1;

                }

            }
            waypoints.Add(path[path.Length - 1]);
            return waypoints.ToArray();

        }
    }
}