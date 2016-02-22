using System.Collections.Generic;
using UnityEngine;

namespace Assets.Pathfinder.Scripts
{
    public class Agent : MonoBehaviour {


        public List<Vector3> path = new List<Vector3>();
        public List<Node> openSet = new List<Node>();
        public List<Node> closedSet = new List<Node>();

        public Pathfinding pf;

        public Transform target;
        public bool drawGizmos;


        void Update () {
            if (Input.GetKeyDown(KeyCode.Space) && pf != null)
            {
                pf.RequestPath(this, target.position);
            }
        }


        void OnDrawGizmos()
        {
            if (!drawGizmos) return;

            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(path[i], path[i + 1]);
            }

            for (int i = 0; i < openSet.Count; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(openSet[i].position, Vector3.one * .5f);
            }
            for (int i = 0; i < closedSet.Count; i++)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireCube(closedSet[i].position, Vector3.one * .5f);

            }
        }

    }
}
