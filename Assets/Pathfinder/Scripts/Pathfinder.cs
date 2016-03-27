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

            Node startNode = graph.GetNode(startPosition);
            Node targetNode = graph.GetNode(targetPosition);

            bool isPathFound = false;
            if (startNode.isWalkable && targetNode.isWalkable)
            {
                Heap<Node> openSet = new Heap<Node>(graph.NodeCount());
                HashSet<Node> closedSet = new HashSet<Node>();

                openSet.Add(startNode);
                while (openSet.Count > 0)
                {
                    Node currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        isPathFound = true;
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

        Vector3[] RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

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

        Vector3[] SimplifyPath(List<Node> path)
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

        int GetDistance(Node nodeA, Node nodeB)
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


