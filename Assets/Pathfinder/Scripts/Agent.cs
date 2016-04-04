using System.Collections.Generic;
using UnityEngine;

namespace Assets.Pathfinder.Scripts
{
    public class Agent : MonoBehaviour {


        public Vector3[] PathArray;


        public Transform Target;
        public bool DrawGizmos;


        void Update () {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PathManager.RequestPath(transform.position, Target.position, OnPathfindingCompleted);
            }
        }


        /// <summary>
        /// Path Request Callback function
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pathfound"></param>
        void OnPathfindingCompleted(Vector3[] path, bool pathfound)
        {
            if (pathfound)
            {
                PathArray = new Vector3[0];
                PathArray = path;
            }
        }


        void OnDrawGizmos()
        {
            if (!DrawGizmos) return;

            for (int i = 0; i < PathArray.Length - 1; i++)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(PathArray[i], PathArray[i + 1]);
            }
            for (int i = 0; i < PathArray.Length - 1; i++)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(PathArray[i] + new Vector3(0,.2f,0), PathArray[i + 1] + new Vector3(0, .2f, 0));
            }

        }

    }
}
