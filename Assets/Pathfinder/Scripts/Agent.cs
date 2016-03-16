using System.Collections.Generic;
using UnityEngine;

namespace Assets.Pathfinder.Scripts
{
    public class Agent : MonoBehaviour {


        public List<Vector3> path = new List<Vector3>();
        public Vector3[] pathArray;

        public Pathfinding pf;

        public Transform target;
        public bool drawGizmos;


        void Update () {
            if (Input.GetKeyDown(KeyCode.Space) && pf != null)
            {
                pf.RequestPath(this, target.position, OnPathfindingCompleted);
            }
        }

        void OnPathfindingCompleted(Vector3[] path, bool pathfound)
        {
            if (pathfound)
            {
                pathArray= new Vector3[0];
                pathArray = path;
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
            for (int i = 0; i < pathArray.Length - 1; i++)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(pathArray[i] + new Vector3(0,.2f,0), pathArray[i + 1] + new Vector3(0, .2f, 0));
            }

        }

    }
}
