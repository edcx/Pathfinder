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

    public List<Graph> graphs = new List<Graph>();

	// Use this for initialization
	void Start () {
        Graph g = new Graph(startPosition, width, height, neighbourCount, edgeLength);
        graphs.Add(g);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator SearchPath(Vector3 start, Vector3 end)
    {
        yield return null;
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
                Gizmos.DrawCube(g.GetNode(i, j).position, new Vector3(0.2f, 0.2f, 0.2f));
            }
        }
    }
}
