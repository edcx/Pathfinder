using System;
using UnityEngine;

namespace Assets.Pathfinder.Scripts
{
    public class Node : IPathable{
        private Vector3  _position;
        private int      _g, _h;
        private int      _x, _y;
        private int      _penalty;
        private bool     _isWalkable;
        private Node     _parent;
        private static int _id = 0;
        private int _heapIndex;


        public Node(Vector3 pos) {
            position    = pos;
            g           = 0; 
            h           = 0;
            _isWalkable  = true;
            id = _id;
            _id++;
        }
        public Node(Vector3 pos, bool walkable, int xCoord, int yCoord, int penalty)
        {
            position = pos;
            _isWalkable = walkable;
            x = xCoord;
            y = yCoord;
            this.penalty = penalty;
            id = _id;
            _id++;
        }




        public int HeapIndex
        {
            get{ return _heapIndex; }
            set { _heapIndex = value; }
        }

        public bool IsWalkable
        {
            get { return _isWalkable; }
            set { _isWalkable = value; }
        }

        public int g { get { return _g; } set { _g = value; } }
        public int h { get { return _h; } set { _h = value; } }

        public int x
        {
            get { return _x; }
            set { _x = value; }
        }

        public int y {
            get { return _y; }
            set { _y = value; }
        }

        public int id { get { return _id; } set { _id = value; } }

        public int penalty
        {
            get { return _penalty; }
            set { _penalty = value; }
        }
        public Vector3 position {
            get { return _position; }
            set { _position = value; }
        }
        public IPathable parent { get; set; }

        public int f { get { return g + h; } }

        public int CompareTo(IPathable other)
        {
            int compare = f.CompareTo(other.f);
            if (compare == 0)
                compare = h.CompareTo(other.h);
            return -compare;
        }
    }


}
