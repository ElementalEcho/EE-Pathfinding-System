using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
namespace EE.PathfindingSystem {
    public class GridContainer : ScriptableObject {
        [SerializeField]
        private Grid grid = new Grid(Vector2.zero, new Vector2(100, 100), 1,10, 1, new TerrainType[0], Connections.directional8DontCutCorners, 1 << 17 | 1 << 15 );

        [SerializeField]
        private Heuristics heuristicMethod = Heuristics.Manhattan;

        [SerializeField]
        private float heuristicMultiplier = 2;


        [SerializeField]
        private TextAsset gridTextFile = null;
        private string storedPath = "";

        public void CreateGrid() {
            if (storedPath.Length <= 0) {
                storedPath = gridTextFile.text;
            }

            grid.CreateGrid(storedPath);

        }
        public bool CheckIfThisFitsToGrid(Vector2 sizeOnGrid, Vector2 spawnPosition) {
            return grid.CheckIfThisFitsToGrid(sizeOnGrid, spawnPosition);

        }
        public void UpdateGridColliders(Vector2 sizeOnGrid, bool walkableBool, Vector2 spawnPosition) {
            var walkable = walkableBool ? NodeType.walkable : NodeType.obstacle;
            grid.UpdateGridColliders(sizeOnGrid, walkable, spawnPosition);
        }
        public Vector2 GetRandomNodePositionFromGrid() {
            return grid.GetRandomNodeFromGrid().WorldPosition;

        }
        public Vector2[] GetPath(Vector2 startPos, Vector2 endPos, bool usePathSmoothing = true) {
            Node start = grid.ClosestNodeFromWorldPoint(startPos);
            Node end = grid.ClosestNodeFromWorldPoint(endPos, start.GridAreaID);
            Vector2[] path = AStar.FindPath(start, end, grid.Maxsize, heuristicMethod, heuristicMultiplier);
            if (usePathSmoothing && path.Length > 0) {
                path = PathfindingUtils.PathSmooter(path, grid.UnwalkableMask);
            }
            return path;
        }



#if UNITY_EDITOR
        public void DrawGriToEditor() {
            grid.DrawGriToEditor();           
        }
        [Button]
        public void ClearString() {
            storedPath = "";
        }
        [Button]
        public void GetPathString() {
            System.IO.File.WriteAllText(UnityEditor.AssetDatabase.GetAssetPath(gridTextFile), grid.GetPathString());
        }

        private PathfindingGrid pathfindingGrid = null;
        [Button]
        public void TestGrid() {
            pathfindingGrid = new GameObject("PathfindingGrid").AddComponent<PathfindingGrid>();
            pathfindingGrid.gridContainer = this;
            storedPath = gridTextFile.text;
            grid.CreateGrid(storedPath);
        }
        [Button]
        public void DeleteGrid() {
            if (pathfindingGrid != null) {
                DestroyImmediate(pathfindingGrid.gameObject);
            }
        }
#endif

    }
    public enum Connections {
        directional4,
        directional8,
        directional8DontCutCorners
    }

}
