using UnityEngine;

namespace Assets.Pathfinder.Scripts
{
    public class Node {
        public Vector3  position;
        public int      g, h;
        public int      x, y;
        public int      penalty;
        public bool     isWalkable;
        public Node     parent;

        public int f 
        {
            get { return g + h; }
        }

        public Node(Vector3 pos) {
            position    = pos;
            g           = 0; 
            h           = 0;
            isWalkable  = true;
        }
        public Node(Vector3 pos, bool walkable, int xCoord, int yCoord, int penalty)
        {
            position = pos;
            isWalkable = walkable;
            x = xCoord;
            y = yCoord;
            this.penalty = penalty;
        }

        public Node DeepCopy()
        {
            Node other = (Node)this.MemberwiseClone();

            other.parent = null;

            return other;
        }



    }
}
