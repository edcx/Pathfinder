using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour {

    public Vector3 startPosition;

    public int width;
    public int height;
    public int neighbourCount = 4;
    public float edgeLength;

    public bool drawGizmos;

    public Transform target;

    public List<Graph> graphs = new List<Graph>();
    public List<Vector3> path = new List<Vector3>();
    public Node endNode;

    PathFindingThread pfThread;
    int threadID = 0;
	// Use this for initialization
	void Start () {
        Graph g = new Graph(startPosition, width, height, neighbourCount, edgeLength);
        graphs.Add(g);
        
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Space))
        {
            pfThread = new PathFindingThread();
            pfThread.Id = threadID;
            pfThread.callBackListener = this;
            pfThread.startPosition = startPosition;
            pfThread.endPosition = target.position;
            pfThread.width = width;
            pfThread.height = height;
            pfThread.neighbourCount = neighbourCount;
            pfThread.graph = graphs[0];

            pfThread.edgeLength = edgeLength;
            pfThread.Start();
            threadID++;
        }
        /*if (pfThread != null)
        {
            if (pfThread.Update())
            {
                // Alternative to the OnFinished callback
                path = pfThread.path;
                pfThread = null;
            }
        }*/
	}

    void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        Gizmos.color = Color.red;
        Graph g = graphs[0];
        for (int i = 0; i < g.width; i++)
        {
            for (int j = 0; j < g.height; j++)
            {
                Gizmos.DrawCube(g.GetNodeAtIndex(i, j).position, new Vector3(0.2f, 0.2f, 0.2f));
            }
        }
       
        for (int i = 0; i < path.Count - 1; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(path[i], path[i+1]);
        }
    }
}
