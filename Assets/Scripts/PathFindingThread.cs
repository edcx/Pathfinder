using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System;

public class PathFindingThread : MultiThread {

    private bool _isPathFound = false;
    private Stopwatch stopWatch;

    public int width;
    public int height;
    public int neighbourCount = 4;
    public float edgeLength;

    public Graph graph;
    public Node endNode;
    
    
    public Vector3 startPosition;
    public Vector3 endPosition;

    public Pathfinding callBackListener;

    public List<Vector3> path = new List<Vector3>();

    protected override void ThreadFunction()
    {
        stopWatch = new Stopwatch();
        stopWatch.Start();
        var openSet = new List<Node>();
        var closedSet = new List<Node>();

        Node startNode = graph.GetNode(startPosition);
        endNode = graph.GetNode(endPosition);

        _isPathFound = false;
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            openSet.Sort(delegate(Node x, Node y) { return x.f.CompareTo(y.f); });
            Node current = openSet[0];

            if (current == endNode)
            {
                //TODO: Path found!
                _isPathFound = true;
                break;
            }
            openSet.RemoveAt(0);
            closedSet.Add(current);

            for (int i = 0; i < neighbourCount; i++)
            {
                if (current.neighbours[i] == null || closedSet.Contains(current.neighbours[i]))
                    continue;
                float tentativeGScore = current.g + CalculateCost(current, current.neighbours[i]);
                if (!openSet.Contains(current.neighbours[i]) || tentativeGScore < current.neighbours[i].g)
                {
                    graph.UpdateNode(current, current.neighbours[i], tentativeGScore, CalculateHeuristic(current.position, current.neighbours[i].position));
                    if (!openSet.Contains(current.neighbours[i]))
                        openSet.Add(current.neighbours[i]);
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

    protected override void OnFinished()
    {
        stopWatch.Stop();
        TimeSpan ts = stopWatch.Elapsed;
        UnityEngine.Debug.Log(">>> PathFinding" + " completed in " + ts.TotalMilliseconds + "ms!");
        if (_isPathFound)
            path = GetPath(endNode);
        if (callBackListener != null)
            callBackListener.path = path;
        Abort();
    }

    float CalculateHeuristic(Vector3 v1, Vector3 v2)
    {
        return Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.z - v2.z) + Mathf.Abs(v1.z - v2.z);
    }
    float CalculateCost(Node n, Node target)
    {
        float cost = 10;
        if ((n.position - target.position).sqrMagnitude >= edgeLength * edgeLength * 2)
            cost = 14;
        return cost;
    }
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
