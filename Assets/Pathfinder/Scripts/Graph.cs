using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Pathfinder.Scripts
{
    public class Graph {
        [Serializable]
        public struct Region
        {
            public LayerMask regionMask;
            public int penalty;
        }

        public Vector3 startPosition;

        public LayerMask unwalkableMask;
        public int width;
        public int height;
        public int neighbourCount = 4;
        public float edgeLength;

        public Node[,] grid;

        public Region[] walkableRegions;
        private LayerMask walkableMask;
        private Dictionary<int, int> walkableRegionsDict = new Dictionary<int, int>(); 
        private Vector2 gridWorldSize;
        private float nodeRadius;


        public Graph(Vector3 startPosition, int width, int height, int neighbourCount, float edgeLength, LayerMask unwalkableMask, Region[] walkableRegions)
        {
            this.startPosition      = startPosition;
            this.width              = width;
            this.height             = height;
            this.neighbourCount     = neighbourCount;
            this.edgeLength         = edgeLength;
            this.unwalkableMask     = unwalkableMask;
            this.walkableRegions            = walkableRegions;

            nodeRadius = edgeLength*.5f;
            gridWorldSize = new Vector2(width,height) * edgeLength;

            foreach (var region in walkableRegions)
            {
                walkableMask.value |= region.regionMask.value;
                walkableRegionsDict.Add((int)Mathf.Log(region.regionMask, 2), region.penalty);

            }

            CreateGrid();
        }

        void CreateGrid()
        {
            grid = new Node[width, height];
            Vector3 worldBottomLeft = startPosition;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * edgeLength + nodeRadius) + Vector3.forward * (y * edgeLength + nodeRadius);
                    bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                    int movementPenalty = 0;

                    if (walkable)
                    {
                        // Apply penalty
                        RaycastHit hit;
                        if (Physics.Raycast(worldPoint + Vector3.up*50f, Vector3.down, out hit, 100f, walkableMask))
                            walkableRegionsDict.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    }

                    grid[x, y] = new Node(worldPoint, walkable, x, y, movementPenalty);
                }
            }
        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    int xIndex = node.x + x;
                    int yIndex = node.y + y;

                    if (xIndex >= 0 && xIndex < width && yIndex >= 0 && yIndex < height)
                        neighbours.Add(grid[xIndex, yIndex]);
                }
            }

            return neighbours;
        }

        public Node GetNode(Vector3 worldPosition)
        {
            float percentX = (worldPosition.x + gridWorldSize.x * .5f) / gridWorldSize.x;
            float percentY = (worldPosition.z + gridWorldSize.y * .5f) / gridWorldSize.y;

            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((width - 1) * percentX);
            int y = Mathf.RoundToInt((height - 1) * percentY);

            return grid[x, y];
        }


        /*
        bool IsInBorders(int i, int j)
        {
            if (i < 0 || i > width - 1) return false;
            if (j < 0 || j > height - 1) return false;
            return true;

        }

        public Node GetNodeAtIndex(int i, int j)
        {
            return nodes[i,j];
        }

        public Node GetNodeAtIndex(int i, int j, Graph g)
        {
            return g.nodes[i,j];
        }
        public Node GetNode(Vector3 pos)
        {
            int i = (int)((pos.x - startPosition.x) / edgeLength);
            int j = (int)((pos.z - startPosition.z) / edgeLength);
            return nodes[i,j];
        }
        public int[] GetIndexOf(Node n)
        { 
            int i = (int)((n.position.x - startPosition.x) / edgeLength);
            int j = (int)((n.position.z - startPosition.z) / edgeLength);
            int[] result = {i,j};
            return result;
        }
 
        /// <summary>
        /// Updates the node.
        /// </summary>
        /// <param name="adj">Adjacent node.</param>
        /// <param name="n">Node.</param>
        public void UpdateNode(Node n, Node neighbour, float cost, float h)
        {
            neighbour.g = cost;
            neighbour.h = h;
            neighbour.parent = n;
            neighbour.f = neighbour.h + neighbour.g;
        }

        /// <summary>
        /// Gets walkable neighbour nodes
        /// </summary>
        /// <param name="n"></param>
        /// <returns>List of existing neighbour nodes</returns>
        public List<Node> GetNeighbours(Node n)
        {
            List<Node> neighbours = new List<Node>();
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if ((i == 0 && j == 0) || (neighbourCount == 4 && Mathf.Abs(i) + Mathf.Abs(j) > 1))
                        continue;
                    int[] nodeIndex = GetIndexOf(n);
                    if (IsInBorders(nodeIndex[0] + i, nodeIndex[1] + j))
                    {
                        neighbours.Add(GetNodeAtIndex(nodeIndex[0] + i, nodeIndex[1] + j));
                    }
                }
            }
            return neighbours;

        }


        */

        public Graph DeepCopy()
        {
            Graph other = (Graph) this.MemberwiseClone();
            other.grid = new Node[width, height];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    other.grid[i,j] = grid[i,j].DeepCopy();
                }
            }

            return other; 
        }


        void GenerateEmptyNodes()
        {
            grid = new Node[width, height];
            for (int i = 0; i < width; i++)
            { 
                for (int j = 0; j < height; j++)
                {
                    grid[i, j] = new Node(startPosition + new Vector3(i, 0, j) * edgeLength);
                }
            }

        }

        void GenerateNotWalkableNodes()
        {
            grid = new Node[width,height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    grid[i,j] = new Node(startPosition + new Vector3(i, 0, j) * edgeLength);
                    if (j == 10 && i > (width * 0.1f) && i < (width * 0.75f))
                        grid[i,j].isWalkable = false;
                }
            }

        }



        
        public void ClearGraph()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    grid[i,j] = new Node(startPosition + new Vector3(width, 0, height) * edgeLength);
                }
            }
        }

        /// <summary>
        /// Clears parent value of all nodes
        /// </summary>
        public void ClearGraphParentData()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    grid[i,j].parent = null;
                }
            }
        }
    }
}
