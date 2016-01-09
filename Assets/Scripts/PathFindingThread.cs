using UnityEngine;
using System.Diagnostics;
using System.Collections.Generic;
using System;

public class PathfindingThread : MultiThread {

    private bool _isPathFound = false;
    private Stopwatch stopWatch;

    public int width;
    public int height;
    public int neighbourCount = 4;
    public float edgeLength;
    public float costModifier;

    public Graph graph;
    public Node endNode;
    

    
    public Vector3 startPosition;
    public Vector3 endPosition;

    public Pathfinding callBackListener;

    public List<Vector3> path = new List<Vector3>();

    List<Node> openSet;
    List<Node> closedSet;

    protected override void ThreadFunction()
    {
        stopWatch = new Stopwatch();
        stopWatch.Start();
        openSet = new List<Node>();
        closedSet = new List<Node>();

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
            //List<Node> neighbours = graph.GetNeighbours(current);

            for (int i = 0; i < neighbourCount; i++)
            {
                if (current.neighbours[i] == null || closedSet.Contains(current.neighbours[i]))
                    continue;
                float tentativeGScore = current.g + CalculateCost(current, current.neighbours[i]);

                if (!openSet.Contains(current.neighbours[i]))
                    openSet.Add(current.neighbours[i]);
                else if (tentativeGScore > current.neighbours[i].g)
                    continue;
                graph.UpdateNode(current, current.neighbours[i], tentativeGScore, CalculateHeuristic(current.neighbours[i].position, endNode.position));
                /*if (neighbours[i] == null || closedSet.Contains(neighbours[i]))
                    continue;
                float tentativeGScore = current.g + CalculateCost(current, neighbours[i]);

                if (!openSet.Contains(neighbours[i]))
                    openSet.Add(neighbours[i]);
                else if (tentativeGScore > neighbours[i].g)
                    continue;
                graph.UpdateNode(current, neighbours[i], tentativeGScore, CalculateHeuristic(neighbours[i].position, endNode.position));*/
            }
            if (callBackListener != null)
            {
                callBackListener.openSet = openSet;
                callBackListener.closedSet = closedSet;
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
        {
            callBackListener.path = path;

        }
        Abort();
    }


    float CalculateDistance2D(Vector3 v1, Vector3 v2)
    {
        return Vector2.Distance(new Vector2(v1.x, v1.z), new Vector2(v2.z, v2.z));
    }

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

    List<Vector3> GetPath(Node n)
    {
        List<Vector3> path = new List<Vector3>();
        if (n == null)
            return path;
        while (n.parent != null)
        {
            //UnityEngine.Debug.Log("Heuristic: " + n.f + " Cost: " + n.g);
            path.Add(n.position);
            n = n.parent;
        }
        //UnityEngine.Debug.Log("Heuristic: " + n.f + " Cost: " + n.g);
        path.Add(n.position);
        return path;

    }
}
