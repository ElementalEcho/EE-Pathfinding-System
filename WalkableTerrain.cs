using System.Collections.Generic;
using UnityEngine;
namespace EE.PathfindingSystem {
    [System.Serializable]
    public class WalkableTerrain {
        public LayerMask walkableMask;
        Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();

        public int GetTerrainPenalty(Vector2 worldPoint, float nodeRadius) {
            int movementPenalty = 0;

            Collider2D[] hit = Physics2D.OverlapCircleAll(worldPoint, nodeRadius, walkableMask);
            for (int i = 0; i < hit.Length; i++) {
                walkableRegionsDictionary.TryGetValue(hit[i].gameObject.layer, out int newPenalty);

                //Return terrain with highest movement penalty
                if (newPenalty > movementPenalty) {
                    movementPenalty = newPenalty;

                }
            }
            return movementPenalty;

        }
        public static WalkableTerrain CreateWalkableTerrain(TerrainType[] walkableRegions) {
            var walkable = new WalkableTerrain();
            foreach (TerrainType region in walkableRegions) {
                walkable.walkableMask.value |= region.terrainMask.value;
                int terrainMask = (int)Mathf.Log(region.terrainMask.value, 2);
                if (walkable.walkableRegionsDictionary.ContainsKey(terrainMask)) {
                    walkable.walkableRegionsDictionary[terrainMask] = region.terrainPenalty;
                }
                else {
                    walkable.walkableRegionsDictionary.Add(terrainMask, region.terrainPenalty);
                }

            }
            return walkable;
        }
    }

    [System.Serializable]
    public class TerrainType {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }
}
