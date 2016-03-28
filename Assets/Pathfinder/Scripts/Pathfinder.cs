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

        private PathManager pathManager;
        private Stopwatch stopWatch;

        [Range(1f, 10f)]
        public float costMultiplier;

        public Graph graph;

        void Awake()
        {
            pathManager = GetComponent<PathManager>();
        }

        public void StartFindPath(Vector3 startPoint, Vector3 endPoint)
        {
            StartCoroutine(FindPath(startPoint, endPoint));
        }

        public IEnumerator FindPath(Vector3 startPosition, Vector3 targetPosition)
        {
            stopWatch = new Stopwatch();
            stopWatch.Start();

            Vector3[] waypoints = new Vector3[0];

            IPathable startNode = graph.GetNode(startPosition);
            IPathable targetNode = graph.GetNode(targetPosition);

            bool isPathFound = false;
            if (startNode.IsWalkable && targetNode.IsWalkable)
            {
                Heap<IPathable> openSet = new Heap<IPathable>(graph.NodeCount());
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
 
                    foreach (IPathable neighbour in graph.GetNeighbours(currentNode))
                    {
                        if (!neighbour.IsWalkable || closedSet.Contains(neighbour)) continue;

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

            if (isPathFound)
            {
                waypoints = RetracePath(startNode, targetNode);
            }
            else
                UnityEngine.Debug.Log("Search path failed!");
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            //UnityEngine.Debug.Log(">>> PathFinding" + " completed in " + ts.TotalMilliseconds + "ms!");

            yield return null;

            pathManager.FinishedProcessingPath(waypoints, isPathFound);
        }

        Vector3[] RetracePath(IPathable startNode, IPathable endNode)
        {
            List<IPathable> path = new List<IPathable>();
            IPathable currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            path.Add(startNode);
            Vector3[] waypoints = SimplifyPath(path);
            Array.Reverse(waypoints);
            return waypoints;
        }

        Vector3[] SimplifyPath(List<IPathable> path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            Vector3 directionOld = Vector3.zero;

            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector3 directionNew = (path[i + 1].position - path[i].position).normalized;
                if (directionNew != directionOld)
                {
                    waypoints.Add(path[i].position);
                }
                directionOld = directionNew;
            }
            waypoints.Add(path[path.Count - 1].position);
            return waypoints.ToArray();
        }

        int GetDistance(IPathable nodeA, IPathable nodeB)
        {
            int distX = Mathf.Abs(nodeA.x - nodeB.x);
            int distY = Mathf.Abs(nodeA.y - nodeB.y);

            if (distX > distY)
                return (int)((14 * distY + 10 * (distX - distY)) * costMultiplier);

            return (int)((14 * distX + 10 * (distY - distX)) * costMultiplier);
        }

        float CalculateDistance2D(Vector3 v1, Vector3 v2)
        {
            return Vector2.Distance(new Vector2(v1.x, v1.z), new Vector2(v2.z, v2.z));
        }

    }
}


