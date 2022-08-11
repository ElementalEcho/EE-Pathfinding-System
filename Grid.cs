using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EE.PathfindingSystem {
    [IncludeMyAttributes]
    [HideLabel]
    [InlineProperty]
    [System.Serializable]
    internal class Grid {
        [SerializeField]
        private Vector2 gridPosition = Vector2.zero;
        [SerializeField]
        private Vector2 gridWorldSize = new Vector2(100, 100);
        [SerializeField]
        private float nodeRadius = 1;
        [SerializeField]
        private float nearestNodeDistance = 10;
        [SerializeField]
        private float collisionRadius = 1;
        [SerializeField]
        private TerrainType[] walkableRegions = new TerrainType[0];
        [SerializeField]
        private Connections options = Connections.directional8DontCutCorners;
        [SerializeField]
        private LayerMask unwalkableMask = 1 << 17 | 1 << 15; // Default obstacle layers, Wall 17 and Objects 15
        public LayerMask UnwalkableMask => unwalkableMask;

        private float nodeDiameter => nodeRadius * 2;
        public int Maxsize => gridSizeX * gridSizeY;
        public Node GetNode(int x, int y) => grid[x, y];

        public bool TryGetNode(int x, int y, out Node node) {
            if (x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY) {
                node = grid[x, y];
                return true;

            }
            node = null;
            return false;
        }
        public void DoActionToAllNodes(Action<int,int> action) {
            for (int x = 0; x < gridSizeX; x++) {
                for (int y = 0; y < gridSizeY; y++) {
                    action.Invoke(x,y);
                }
            }
        }


        protected int numberOfGridAreas = 1;
        protected Node[,] grid;
        private int gridSizeX;
        public int GridSizeX => gridSizeX;
        private int gridSizeY;
        public int GridSizeY => gridSizeY;
        protected WalkableTerrain _walkableTerrain;

        public Grid(Vector2 gridPosition, Vector2 gridWorldSize, float nodeRadius, float nearestNodeDistance, float collisionRadius, TerrainType[] walkableRegions, Connections options, LayerMask unwalkableMask) {
            this.gridPosition = gridPosition;
            this.gridWorldSize = gridWorldSize;
            this.nodeRadius = nodeRadius;
            this.nearestNodeDistance = nearestNodeDistance;
            this.collisionRadius = collisionRadius;
            this.walkableRegions = walkableRegions;
            this.options = options;
            this.unwalkableMask = unwalkableMask;
        }


        public void CreateGrid(string storedPath) {
            _walkableTerrain = WalkableTerrain.CreateWalkableTerrain(walkableRegions);
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

#if UNITY_EDITOR
            if (storedPath.Length != Maxsize) {
                Debug.LogError($"Stored Path is not valid for this grid. Required size : {Maxsize} ,Stored Path Size: {storedPath.Length}");
                return;
            }
#endif
            CreateNodes(storedPath);
            SetNeighbours();
            SetAreas();
        }
        protected void CreateNodes(string storedPath) {
            grid = new Node[gridSizeX, gridSizeY];
            Vector2 worldBottomLeft =  new(gridPosition.x - gridWorldSize.x / 2, gridPosition.y - gridWorldSize.y / 2);

            void CreateNode(int x, int y) {
                int currentIndex = y + (x * gridSizeY);

                Vector2 worldPoint = new(worldBottomLeft.x + (x * nodeDiameter + nodeRadius), worldBottomLeft.y + (y * nodeDiameter + nodeRadius));
                NodeType nodeType = storedPath[currentIndex] == '0' ? NodeType.walkable : NodeType.obstacle;
                int movementPenalty = nodeType == NodeType.walkable ? _walkableTerrain.GetTerrainPenalty(worldPoint, nodeRadius) : 0;

                grid[x, y] = new Node(nodeType, worldPoint, x, y, movementPenalty);
                currentIndex++;
            }

            DoActionToAllNodes(CreateNode);
        }
        private void SetNeighbours() {
            void SetNeightBour(int x, int y) {
                var node = GetNode(x, y);
                SetNeighbours(node);
            }
            DoActionToAllNodes(SetNeightBour);
        }

        private void SetNeighbours(Node node) {
            Node newNode;

            int checkX;
            int checkY;
            int index = 0;

            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    //Skip center node, because it is current node
                    if (x == 0 && y == 0)
                        continue;

                    if (options.Equals(Connections.directional4) && (Mathf.Abs(x) + Mathf.Abs(y) == 2)) {
                        continue;
                    }

                    checkX = node.GridX + x;
                    checkY = node.GridY + y;

                    if (checkX >= 0 && checkX < GridSizeX && checkY >= 0 && checkY < GridSizeY) {
                        newNode = GetNode(checkX, checkY);

                        if (node.Parent == newNode) {
                            continue;
                        }

                        //Prevent corner cutting
                        var neighbour2 = GetNode(checkX, node.GridY);
                        var neighbour3 = GetNode(node.GridX, checkY);

                        if (options.Equals(Connections.directional8DontCutCorners) && (!newNode.Walkable || !neighbour2.Walkable || !neighbour3.Walkable)) {
                            continue;
                        }
                        else {
                            node.Neighbours[index] = newNode;
                            index++;
                        }
                    }
                }
            }
        }

        private void SetAreas() {
            void UpdateGridAreas(int x, int y) {
                var node = GetNode(x, y);
                if (node.IsWalkable(0)) {
                    Heap<Node> openSet = new Heap<Node>(Maxsize);
                    Heap<Node> closedList = new Heap<Node>(Maxsize);

                    node.SetGridArea(numberOfGridAreas);
                    openSet.Add(node);

                    Node neighbour;
                    Node currentNode;

                    while (openSet.Count > 0) {
                        currentNode = openSet.RemoveFirst();
                        closedList.Add(currentNode);

                        for (int i = 0; i < currentNode.Neighbours.Length; i++) {
                            neighbour = currentNode.Neighbours[i];

                            if (neighbour == null || !neighbour.Walkable || closedList.Contains(neighbour)) {
                                continue;
                            }
                            if (openSet.Contains(neighbour) == false) {
                                neighbour.SetGridArea(numberOfGridAreas);
                                openSet.Add(neighbour);
                            }

                        }
                    }
                    numberOfGridAreas++;
                }
            }
            DoActionToAllNodes(UpdateGridAreas);
        }

        public Node ClosestNodeFromWorldPoint(Vector2 worldPosition, int nodeArea = -1) {
            var node = NodeFromWorldPoint(worldPosition);
            //If target node is inside collider return nearby node
            if (!node.Walkable || (nodeArea >= 0 && node.GridAreaID != nodeArea)) {
                Node neighbour = FindWalkableInRadius(node.GridX, node.GridY, 1, nodeArea);
                if (neighbour != null) {
                    return neighbour;
                }
            }
            return node;
        }

        private Node NodeFromWorldPoint(Vector2 worldPosition) {
            float positionOfNodeInGridX = (worldPosition.x - gridPosition.x);
            float positionOfNodeInGridY = (worldPosition.y - gridPosition.y);
            float percentX = (positionOfNodeInGridX + gridWorldSize.x / 2) / gridWorldSize.x;
            float percentY = (positionOfNodeInGridY + gridWorldSize.y / 2) / gridWorldSize.y;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);
            int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
            return GetNode(x, y);
        }

        private Node FindWalkableInRadius(int centreX, int centreY, int radius, int nodeArea) {
            if (radius > nearestNodeDistance) {
                Debug.LogWarning("Target area is not in nearestNodeDistance!");
                return null;
            }
            for (int i = -radius; i <= radius; i++) {
                int verticalSearchX = i + centreX;
                int horizontalSearchY = i + centreY;

                Node node;
                // top
                if (TryGetNode(verticalSearchX, centreY + radius, out node) && node.IsWalkable(nodeArea)) {
                    return node;                   
                }
                // bottom
                if (TryGetNode(verticalSearchX, centreY - radius, out node) && node.IsWalkable(nodeArea)) {
                    return node;
                }
                // right
                if(TryGetNode(centreY + radius, horizontalSearchY, out node) && node.IsWalkable(nodeArea)) {
                    return node;
                }
                // left
                if(TryGetNode(centreY - radius, horizontalSearchY, out node) && node.IsWalkable(nodeArea)) {
                    return node;
                }
            }
            radius++;
            return FindWalkableInRadius(centreX, centreY, radius, nodeArea);

        }


        private void UpdateCollider(Vector2 worldPosition, int radius) {
            Node node = NodeFromWorldPoint(worldPosition);

            for (int x = -radius; x <= radius; x++) {
                for (int y = -radius; y <= radius; y++) {
                    var neighbour = GetNode(node.GridX + x, node.GridY + y);
                    UpdateNodeColliders(neighbour, nodeRadius, collisionRadius,unwalkableMask);
                }
            }
        }

        private void UpdateNodeColliders(Node node,float nodeRadius, float collisionRadius, LayerMask unwalkableMask) {
            ////Calculate obstacles while creating path
            Collider2D[] colliders = Physics2D.OverlapCircleAll(node.WorldPosition, nodeRadius * collisionRadius, unwalkableMask);
            var walkable = colliders.Length > 0 ? NodeType.obstacle : NodeType.walkable;
            node.SetWalkable(walkable);
        }

        public Node GetRandomNodeFromGrid() {
            int randomX = Mathf.RoundToInt(Random.Range(0, gridWorldSize.x - 1));
            int randomY = Mathf.RoundToInt(Random.Range(0, gridWorldSize.y - 1));
            return GetNode(randomX, randomY);
        }

        public bool CheckIfThisFitsToGrid(Vector2 sizeOnGrid, Vector2 spawnPosition) {
            Node nod = NodeFromWorldPoint(spawnPosition);
            for (int i = (int)-sizeOnGrid.x + 1; i < sizeOnGrid.x + 1; i++) {
                for (int j = (int)-sizeOnGrid.y + 1; j < sizeOnGrid.y + 1; j++) {
                    if (nod.GridX + i >= grid.GetLength(0) || nod.GridY + j >= grid.GetLength(1)) {
                        Debug.LogError("Object out of bounds!");
                        return false;
                    }
                    var neigbour = GetNode(nod.GridX + i, nod.GridY + j);

                    if (!neigbour.Walkable) {
                        return false;
                    }
                }
            }
            return true;
        }

        public void UpdateGridColliders(Vector2 sizeOnGrid, NodeType walkable, Vector2 spawnPosition) {
            Node node = NodeFromWorldPoint(spawnPosition);
            for (int i = (int)-sizeOnGrid.x + 1; i < sizeOnGrid.x + 1; i++) {
                for (int j = (int)-sizeOnGrid.y + 1; j < sizeOnGrid.y + 1; j++) {

                    if (node.GridX + i >= grid.GetLength(0) || node.GridY + j >= grid.GetLength(1)) {
                        Debug.LogError("Object out of bounds!");
                        return;
                    }
                    var neigbour = GetNode(node.GridX + i, node.GridY + j);
                    neigbour.SetWalkable(walkable);
                }
            }
        }

        public string GetPathString() {
            var stringBuilder = new StringBuilder();
            Vector2 worldBottomLeft = new(gridPosition.x - gridWorldSize.x / 2, gridPosition.y - gridWorldSize.y / 2);

            void CreatePathString(int x, int y) {
                int currentIndex = y + (x * gridSizeY);

                Vector2 worldPoint = new(worldBottomLeft.x + (x * nodeDiameter + nodeRadius), worldBottomLeft.y + (y * nodeDiameter + nodeRadius));


                bool walkable = (Physics2D.OverlapCircle(worldPoint, nodeRadius * collisionRadius, unwalkableMask) == null);
                var text = walkable ? 0 : 1;
                stringBuilder.Append(text);
            }

            DoActionToAllNodes(CreatePathString);

            return stringBuilder.ToString();
        }

#if UNITY_EDITOR
        public void DrawGriToEditor() {
            if (grid != null) {
                Gizmos.DrawWireCube(gridPosition, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

                foreach (Node n in grid) {
                    var color = 255 - (255f / 100f * n.MovementPenalty);
                    Gizmos.color = (n.Walkable) ? new Color(color, color, color, 0.4f) : new Color(255, 0, 0, 0.4f);
                    Gizmos.DrawCube(n.WorldPosition, Vector2.one * (nodeDiameter - .1f));

                }

            }
        }
#endif
    }
}
