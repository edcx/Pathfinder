using UnityEngine;
using System.Collections;

public class Graph {

    public Vector3 startPosition;

    public int width;
    public int height;
    public int neighbourCount = 4;
    public float edgeLength;

    private Node[][] nodes;

    public Graph(Vector3 startPosition, int width, int height, int neighbourCount, float edgeLength)
    {
        this.startPosition      = startPosition;
        this.width              = width;
        this.height             = height;
        this.neighbourCount     = neighbourCount;
        this.edgeLength         = edgeLength;

        GenerateEmptyNodes();
    }

    float CalculateHeuristic(Vector3 v1, Vector3 v2)
    {
        return Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.z - v2.z) + Mathf.Abs(v1.z - v2.z);
    }


    bool IsInBorders(int i, int j)
    {
        if (i < 0 || i > width - 1) return false;
        if (j < 0 || j > height - 1) return false;
        return true;

    }

    public Node GetNode(int i, int j)
    {
        return nodes[i][j];
    }

    void GenerateEmptyNodes()
    {
        nodes = new Node[width][];
        for (int i = 0; i < width; i++)
        { 
            nodes[i] = new Node[height];
            for (int j = 0; j < height; j++)
            {
                nodes[i][j] = new Node();
                nodes[i][j].position = startPosition + new Vector3(i, 0, j) * edgeLength;
            }
        }
    }

    public void ClearGraph()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                nodes[i][j] = new Node();
                nodes[i][j].position = startPosition + new Vector3(width, 0, height) * edgeLength;
            }
        }
    }
}
