using System.Collections.Generic;
using UnityEngine;

namespace Assets.Pathfinder.Scripts
{
    public class Grid2D : Graph {

        public Vector3 StartPosition;

        public LayerMask UnwalkableMask;
        public int Width;
        public int Height;
        public float EdgeLength;
        public float AgentHeight = 2;
        public Node[,] Grid;

        public Region[] WalkableRegions;

        public bool DrawGizmos;


        private LayerMask _walkableMask;
        private readonly Dictionary<int, int> _walkableRegionsDict = new Dictionary<int, int>();
        private Vector2 _gridWorldSize;
        private float _nodeRadius;

        void Awake()
        {
            ConstructGrid2D(StartPosition, Width, Height, EdgeLength, AgentHeight, UnwalkableMask, WalkableRegions);
        }

        /// <summary>
        /// Initialize 2D grid in 3D world
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="edgeLength"></param>
        /// <param name="agentHeight"></param>
        /// <param name="unwalkableMask"></param>
        /// <param name="walkableRegions"></param>
        public void ConstructGrid2D(Vector3 startPosition, int width, int height,  float edgeLength, float agentHeight, LayerMask unwalkableMask, Region[] walkableRegions)
        {
            StartPosition = startPosition;
            Width = width;
            Height = height;
            EdgeLength = edgeLength;
            UnwalkableMask = unwalkableMask;
            WalkableRegions = walkableRegions;
            AgentHeight = agentHeight;

            _nodeRadius = edgeLength * .5f;
            _gridWorldSize = new Vector2(width, height) * edgeLength;

            foreach (var region in walkableRegions)
            {
                _walkableMask.value |= region.RegionMask.value;
                _walkableRegionsDict.Add((int)Mathf.Log(region.RegionMask, 2), region.Penalty);

            }

            CreateGrid();
        }

        void CreateGrid()
        {
            Grid = new Node[Width, Height];
            Vector3 worldBottomLeft = StartPosition;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {

                    int movementPenalty = 0;
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * EdgeLength + _nodeRadius) + Vector3.forward * (y * EdgeLength + _nodeRadius);
                    RaycastHit hit;
                    Vector3 groundPoint = worldPoint;
                    bool walkable = false;
                    if (Physics.Raycast(worldPoint + Vector3.up * 50f, Vector3.down, out hit, 100f, _walkableMask))
                    {
                        _walkableRegionsDict.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                        groundPoint = hit.point;
                        walkable = !(Physics.CheckCapsule(groundPoint, groundPoint + Vector3.up * AgentHeight, _nodeRadius, UnwalkableMask));
                    }
                    /*Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * edgeLength + nodeRadius) + Vector3.forward * (y * edgeLength + nodeRadius);
                    bool walkable = !(Physics.CheckCapsule(worldPoint, worldPoint + Vector3.up * agentHeight, nodeRadius, unwalkableMask));

                    int movementPenalty = 0;

                    if (walkable)
                    {
                        // Apply penalty
                        RaycastHit hit;
                        if (Physics.Raycast(worldPoint + Vector3.up*50f, Vector3.down, out hit, 100f, walkableMask))
                            walkableRegionsDict.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    }*/
                    //if (walkable)
                    Grid[x, y] = new Node(groundPoint, walkable, x, y, movementPenalty);
                }
            }
        }

        public override List<IPathable> GetNeighbours(IPathable node)
        {
            List<IPathable> neighbours = new List<IPathable>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    int xIndex = node.X + x;
                    int yIndex = node.Y + y;

                    // Check if neighbour is in borders
                    if (xIndex >= 0 && xIndex < Width && yIndex >= 0 && yIndex < Height)
                    {
                        if (Grid[xIndex, yIndex] == null) continue;
                        if (Mathf.Abs(Grid[xIndex, yIndex].Position.y - node.Position.y) < 1.5f) // TODO: Remove Magic Number!
                            neighbours.Add(Grid[xIndex, yIndex]);
                    }
                }
            }

            return neighbours;
        }

        public override int NodeCount()
        {
            return Width*Height;
        }

        public override IPathable GetNode(Vector3 worldPosition)
        {
            float percentX = (worldPosition.x - StartPosition.x) / _gridWorldSize.x;
            float percentY = (worldPosition.z - StartPosition.z) / _gridWorldSize.y;

            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((Width - 1) * percentX);
            int y = Mathf.RoundToInt((Height - 1) * percentY);

            return Grid[x, y];
        }

        void OnDrawGizmos()
        {
            if (!DrawGizmos) return;

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Gizmos.color = Color.yellow;
                    if (Grid[i, j] == null) continue;
                    if (!Grid[i, j].IsWalkable)
                        Gizmos.color = Color.red;
                    Gizmos.DrawCube(Grid[i, j].Position, Vector3.one * EdgeLength * .5f);

                }
            }

        }
    }
}
