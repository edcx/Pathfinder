using UnityEngine;

namespace Assets.Pathfinder.Scripts
{
    public class Node {
        public Vector3  position;
        public float    f, g, h;
        public bool     isWalkable;
        public Node     parent;

        public Node() {
            position    = Vector3.zero;
            f           = 0; 
            g           = 0; 
            h           = 0;
            isWalkable  = true;
            parent      = null;
        }
        public Node(int neighbourCount)
        {
            position = Vector3.zero;
            f = 0;
            g = 0;
            h = 0;
            isWalkable = true;
            parent = null;
        }

        public Node DeepCopy()
        {
            Node other = (Node)this.MemberwiseClone();

            other.parent = null;

            return other;
        }



    }
}
