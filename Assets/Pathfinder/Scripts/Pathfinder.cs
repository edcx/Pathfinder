using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Assets.Pathfinder.Scripts
{
    public class Pathfinder : MonoBehaviour
    {

        private PathManager _pathManager;
        private Stopwatch _stopWatch;

        [Range(1f, 10f)]
        public float CostMultiplier;

        public Graph Graph;

        void Awake()
        {
            _pathManager = GetComponent<PathManager>();
        }

        public void StartFindPath(Vector3 startPoint, Vector3 endPoint)
        {
            StartCoroutine(FindPath(startPoint, endPoint));
        }

        /// <summary>
        /// Search for a path.
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        public IEnumerator FindPath(Vector3 startPosition, Vector3 targetPosition)
        {
            _stopWatch = new Stopwatch();
            _stopWatch.Start();

            Vector3[] waypoints = new Vector3[0];

            IPathable startNode = Graph.GetNode(startPosition);
            IPathable targetNode = Graph.GetNode(targetPosition);

            bool isPathFound = false;
            if (startNode.IsWalkable && targetNode.IsWalkable)
            {
                Heap<IPathable> openSet = new Heap<IPathable>(Graph.NodeCount());
                HashSet<IPathable> closedSet = new HashSet<IPathable>();

                openSet.Add(startNode);
                while (openSet.Count > 0)
                {
                    IPathable currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        isPathFound = true;
                        break;
                    }
 
                    foreach (IPathable neighbour in Graph.GetNeighbours(currentNode))
                    {
                        if (!neighbour.IsWalkable || closedSet.Contains(neighbour)) continue;

                        int newMovementCostToNeighbour = currentNode.G + GetDistance(currentNode, neighbour) + neighbour.Penalty;
                        if (newMovementCostToNeighbour < neighbour.G || !openSet.Contains(neighbour))
                        {
                            neighbour.G = newMovementCostToNeighbour;
                            neighbour.H = GetDistance(neighbour, targetNode);
                            neighbour.Parent = currentNode;
                            if (!openSet.Contains(neighbour))
                                openSet.Add(neighbour);
                            else
                                openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }

            if (isPathFound)
            {
                waypoints = RetracePath(startNode, targetNode);
            }
            else
                UnityEngine.Debug.Log("Search path failed!");
            _stopWatch.Stop();
            TimeSpan ts = _stopWatch.Elapsed;
            //UnityEngine.Debug.Log(">>> PathFinding" + " completed in " + ts.TotalMilliseconds + "ms!");

            yield return null;

            _pathManager.FinishedProcessingPath(waypoints, isPathFound);
        }

        Vector3[] RetracePath(IPathable startNode, IPathable endNode)
        {
            List<IPathable> path = new List<IPathable>();
            IPathable currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            path.Add(startNode);
            Vector3[] waypoints = SimplifyPath(path);
            Array.Reverse(waypoints);
            return waypoints;
        }

        /// <summary>
        /// Removes nodes if we are on a straigth path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Vector3[] SimplifyPath(List<IPathable> path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            Vector3 directionOld = Vector3.zero;

            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector3 directionNew = (path[i + 1].Position - path[i].Position).normalized;
                if (directionNew != directionOld)
                {
                    waypoints.Add(path[i].Position);
                }
                directionOld = directionNew;
            }
            waypoints.Add(path[path.Count - 1].Position);
            return waypoints.ToArray();
        }

        /// <summary>
        /// Returns distance from first point to second
        /// </summary>
        /// <param name="nodeA"></param>
        /// <param name="nodeB"></param>
        /// <returns></returns>
        int GetDistance(IPathable nodeA, IPathable nodeB)
        {
            int distX = Mathf.Abs(nodeA.X - nodeB.X);
            int distY = Mathf.Abs(nodeA.Y - nodeB.Y);

            //TODO: This works for grids! Make it more general
            
            // Horizontal and vertical movement cost 10
            // Diagonal movement cost 14
            if (distX > distY)
                return (int)((14 * distY + 10 * (distX - distY)) * CostMultiplier);

            return (int)((14 * distX + 10 * (distY - distX)) * CostMultiplier);
        }

    }
}


