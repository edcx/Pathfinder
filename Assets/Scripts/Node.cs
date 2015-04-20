﻿using UnityEngine;
using System.Collections;

public class Node {
    public Vector3  position;
    public float    f, g, h;
    public bool     isWalkable;
    public Node     parent;
    public Node[]   neighbours;

    public Node() {
        position    = Vector3.zero;
        f           = 0; 
        g           = 0; 
        h           = 0;
        isWalkable  = false;
        parent      = null;
    }
    public Node(int neighbourCount)
    {
        position = Vector3.zero;
        f = 0;
        g = 0;
        h = 0;
        isWalkable = false;
        parent = null;
        neighbours = new Node[neighbourCount];
    }
}
