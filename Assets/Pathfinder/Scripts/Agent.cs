using System.Collections.Generic;
using UnityEngine;

namespace Assets.Pathfinder.Scripts
{
    public class Agent : MonoBehaviour {


        public List<Vector3> path = new List<Vector3>();

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

        }

    }
}
