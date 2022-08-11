using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using Sirenix.OdinInspector;

namespace EE.PathfindingSystem {



    public class PathfindingGrid : MonoBehaviour {
        public bool createGrid = false;
        public bool showGrid = true;
        [InlineEditor]
        public GridContainer gridContainer;


        public void Awake() {
            if (createGrid) {
                gridContainer.CreateGrid();
            }

        }
#if UNITY_EDITOR
        void OnDrawGizmos() {
            if (showGrid) {
                gridContainer.DrawGriToEditor();
            }
        }
#endif


    }
}