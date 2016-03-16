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

        struct PathRequest
        {
            public Vector3 pathStart;
            public Vector3 pathEnd;
            public Action<Vector3[], bool> callback;

            public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
            {
                pathStart = _start;
                pathEnd = _end;
                callback = _callback;
            }
        }

        Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
        private PathRequest currentPathRequest;

        private bool isProcessingPath;


        private Stopwatch stopWatch;

        public int width;
        public int height;
        public int neighbourCount = 4;
        public float edgeLength;
        public float costModifier;

        public Graph graph;

        //public List<Vector3> path = new List<Vector3>();

        private static Pathfinder instance;

        public Pathfinder()
        {
            instance = this;

        }

        public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
        {
            PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
            instance.pathRequestQueue.Enqueue(newRequest);
            instance.TryProcessNext();
        }

        public static int ReturnQueuedRequestCount()
        {
            return instance.pathRequestQueue.Count;
        }
        void TryProcessNext()
        {
            if (!isProcessingPath && pathRequestQueue.Count > 0)
            {
                currentPathRequest = pathRequestQueue.Dequeue();
                isProcessingPath = true;
                StartCoroutine(CalculatePath(currentPathRequest.pathStart, currentPathRequest.pathEnd));
                //IEnumerator e = CalculatePath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
                //while (e.MoveNext()){ }

            }
        }

        public void FinishedProcessingPath(Vector3[] path, bool success)
        {
            currentPathRequest.callback(path, success);
            isProcessingPath = false;
            TryProcessNext();
        }

        public IEnumerator CalculatePath(Vector3 startPosition, Vector3 targetPosition)
        {
            stopWatch = new Stopwatch();
            stopWatch.Start();

            Vector3[] waypoints = new Vector3[0];

            Node startNode = graph.GetNode(startPosition);
            Node targetNode = graph.GetNode(targetPosition);

            bool isPathFound = false;
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

            FinishedProcessingPath(waypoints, isPathFound);
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
            path.Reverse();
            Vector3[] waypoints = SimplifyPath(path);
            //Array.Reverse(waypoints);
            return waypoints;
        }

        Vector3[] SimplifyPath(List<Node> path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;

            for (int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew = new Vector2(path[i - 1].x - path[i].x, path[i - 1].y - path[i].y);
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
                return 14 * distY + 10 * (distX - distY);

            return 14 * distX + 10 * (distY - distX);
        }

        float CalculateDistance2D(Vector3 v1, Vector3 v2)
        {
            return Vector2.Distance(new Vector2(v1.x, v1.z), new Vector2(v2.z, v2.z));
        }

    }
}


