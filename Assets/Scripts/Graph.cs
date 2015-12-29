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

        //GenerateEmptyNodes();
        GenerateNotWalkableNodes();
    }



    bool IsInBorders(int i, int j)
    {
        if (i < 0 || i > width - 1) return false;
        if (j < 0 || j > height - 1) return false;
        return true;

    }

    public Node GetNodeAtIndex(int i, int j)
    {
        return nodes[i][j];
    }
    public Node GetNode(Vector3 pos)
    {
        int i = (int)((pos.x - startPosition.x) / edgeLength);
        int j = (int)((pos.z - startPosition.z) / edgeLength);
        return nodes[i][j];
    }
    public int[] GetIndexOf(Node n)
    { 
        int i = (int)((n.position.x - startPosition.x) / edgeLength);
        int j = (int)((n.position.z - startPosition.z) / edgeLength);
        int[] result = {i,j};
        return result;
    }
    /// <summary>
    /// Updates the node.
    /// </summary>
    /// <param name="adj">Adjacent node.</param>
    /// <param name="n">Node.</param>
    public void UpdateNode(Node n, Node neighbour, float cost, float h)
    {
        neighbour.g = cost;
        neighbour.h = h;
        neighbour.parent = n;
        neighbour.f = neighbour.h + neighbour.g;
    }

    void GenerateEmptyNodes()
    {
        nodes = new Node[width][];
        for (int i = 0; i < width; i++)
        { 
            nodes[i] = new Node[height];
            for (int j = 0; j < height; j++)
            {
                nodes[i][j] = new Node(neighbourCount);
                nodes[i][j].position = startPosition + new Vector3(i, 0, j) * edgeLength;
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                SetNeighbours(nodes[i][j]);
            }
        }
    }

    void GenerateNotWalkableNodes()
    {
        nodes = new Node[width][];
        for (int i = 0; i < width; i++)
        {
            nodes[i] = new Node[height];
            for (int j = 0; j < height; j++)
            {
                nodes[i][j] = new Node(neighbourCount);
                nodes[i][j].position = startPosition + new Vector3(i, 0, j) * edgeLength;
                if (j == 10 && i > (width * 0.1f) && i < (width * 0.75f))
                    nodes[i][j].isWalkable = false;
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                SetNeighbours(nodes[i][j]);
            }
        }
    }

    public void SetNeighbours(Node n)
    {
        int index = 0;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if ((i == 0 && j == 0) ||(neighbourCount == 4 && Mathf.Abs(i) + Mathf.Abs(j) > 1))
                    continue;
                int[] nodeIndex = GetIndexOf(n);
                if (IsInBorders(nodeIndex[0] + i, nodeIndex[1] + j))
                {
                    if (!GetNodeAtIndex(nodeIndex[0] + i, nodeIndex[1] + j).isWalkable) continue;

                    n.neighbours[index] = GetNodeAtIndex(nodeIndex[0] + i, nodeIndex[1] + j);
                    index++;
                }
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
