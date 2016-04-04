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
        private static int _id = 0;
        private int _heapIndex;


        public Node(Vector3 pos) {
            _position    = pos;
            _g           = 0; 
            _h           = 0;
            _isWalkable  = true;
            _id++;
        }
        public Node(Vector3 pos, bool walkable, int xCoord, int yCoord, int penalty)
        {
            _position = pos;
            _isWalkable = walkable;
            _x = xCoord;
            _y = yCoord;
            _penalty = penalty;
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

        public int G { get { return _g; } set { _g = value; } }
        public int H { get { return _h; } set { _h = value; } }
        public int F { get { return _g + _h; } }

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        public int Y {
            get { return _y; }
            set { _y = value; }
        }

        public int Id { get { return _id; } set { _id = value; } }

        public int Penalty
        {
            get { return _penalty; }
            set { _penalty = value; }
        }
        public Vector3 Position {
            get { return _position; }
            set { _position = value; }
        }
        public IPathable Parent { get; set; }


        public int CompareTo(IPathable other)
        {
            int compare = F.CompareTo(other.F);
            if (compare == 0)
                compare = H.CompareTo(other.H);
            return -compare;
        }
    }


}
