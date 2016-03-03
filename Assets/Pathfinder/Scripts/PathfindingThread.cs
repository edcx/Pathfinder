using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Pathfinder.Scripts
{
    public class PathfindingThread : MultiThread {

        private bool _isPathFound = false;
        private Stopwatch stopWatch;

        public int width;
        public int height;
        public int neighbourCount = 4;
        public float edgeLength;
        public float costModifier;

        public Graph graph;
        public Node targetNode;
        public Node startNode;
    

    
        public Vector3 startPosition;
        public Vector3 targetPosition;

        public Agent callBackListener;

        public List<Vector3> path = new List<Vector3>();


        //TODO: could use array for storing these with assigning nodeIndex to each node


        protected override void ThreadFunction()
        {
            stopWatch = new Stopwatch();
            stopWatch.Start();


            startNode = graph.GetNode(startPosition);
            targetNode = graph.GetNode(targetPosition);

            _isPathFound = false;
            if (startNode.isWalkable && targetNode.isWalkable)
            {
                Heap<Node> openSet = new Heap<Node>(graph.width * graph.height);
                HashSet<Node> closedSet = new HashSet<Node>();

                openSet.Add(startNode);
                while (openSet.Count > 0)
                {
                    Node currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        _isPathFound = true;
                        break;
                    }
                    foreach (Node neighbour in graph.GetNeighbours(currentNode))
                    {
                        if (!neighbour.isWalkable || closedSet.Contains(neighbour)) continue;

                        int newMovementCostToNeighbour = currentNode.g + GetDistance(currentNode, neighbour) + neighbour.penalty;
                        if (newMovementCostToNeighbour < neighbour.g || !openSet.Contains(neighbour))
                        {
                            neighbour.g = newMovementCostToNeighbour;
                            neighbour.h = GetDistance(neighbour, targetNode);
                            neighbour.parent = currentNode;
                            if (!openSet.Contains(neighbour))
                                openSet.Add(neighbour);
                            else
                                openSet.UpdateItem(neighbour);
                        }

                    }
                }
            }
            if (_isPathFound)
            {
                UnityEngine.Debug.Log("Path found!");
            }
            else
                UnityEngine.Debug.Log("Search path failed!");
            OnFinished();

        }
        List<Vector3> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            path.Add(startNode);

            List<Vector3> waypoints = SimplifyPath(path);
            //Array.Reverse(waypoints);
            waypoints.Reverse();
            return waypoints;
        }

        List<Vector3> SimplifyPath(List<Node> path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;

            for (int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew = new Vector2(path[i - 1].x - path[i].x, path[i - 1].y - path[i].y);
                //if (directionNew != directionOld)
                {
                    waypoints.Add(path[i].position);
                }
                directionOld = directionNew;
            }
            return waypoints;
        }

        int GetDistance(Node nodeA, Node nodeB)
        {
            int distX = Mathf.Abs(nodeA.x - nodeB.x);
            int distY = Mathf.Abs(nodeA.y - nodeB.y);

            if (distX > distY)
                return 14 * distY + 10 * (distX - distY);

            return 14 * distX + 10 * (distY - distX);
        }

        protected void OnFinished()
        {
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            UnityEngine.Debug.Log(">>> PathFinding" + " completed in " + ts.TotalMilliseconds + "ms!");
            if (_isPathFound)
                path = RetracePath(startNode, targetNode);
            if (callBackListener != null)
            {
                callBackListener.path = path;
            }
            Abort();
        }



        float CalculateDistance2D(Vector3 v1, Vector3 v2)
        {
            return Vector2.Distance(new Vector2(v1.x, v1.z), new Vector2(v2.z, v2.z));
        }

        /// <summary>
        /// Calculates heuristic value for A*
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        float CalculateHeuristic(Vector3 v1, Vector3 v2)
        {
            //Manhattan Distance
            //dx = abs(node.x - goal.x)
            //dy = abs(node.y - goal.y)
            //return D * (dx + dy)         ----> Choose D as the lowest cost between adjacent squares

            // Diagonal Distance
            //dx = abs(node.x - goal.x)
            //dy = abs(node.y - goal.y)
            //return D * (dx + dy) + (D2 - 2 * D) * min(dx, dy)

            //When D = 1 and D2 = 1, this is called the Chebyshev distance. When D = 1 and D2 = sqrt(2), this is called the octile distance
            var dx = Mathf.Abs(v1.x - v2.x);
            var dy = Mathf.Abs(v1.z - v2.z);
        
            if (neighbourCount == 8) 
                return (dx + dy) - Mathf.Min(dx, dy);

            return (dx + dy);
        
            //return Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.z - v2.z) + Mathf.Abs(v1.z - v2.z);
        }

        float CalculateCost(Node n, Node target)
        {

            // g'(n) = 1 + alpha * (g(n) - 1)
            float cost = edgeLength;
            if ((n.position - target.position).sqrMagnitude >= edgeLength * edgeLength * 2)
                cost = edgeLength * 1.4f;
            //return cost;
            return edgeLength + costModifier * (cost - edgeLength);
        }

        /// <summary>
        /// Traces back to start node to get found path
        /// </summary>
        /// <param name="n"></param>
        /// <returns>List of positions on the path</returns>
        List<Vector3> GetPath(Node n)
        {
            List<Vector3> path = new List<Vector3>();
            if (n == null)
                return path;
            while (n.parent != null)
            {
                path.Add(n.position);
                n = n.parent;
            }
            path.Add(n.position);
            return path;

        }
    }
}
