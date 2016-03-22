using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Pathfinder.Scripts
{
    public class PathManager : MonoBehaviour {

        public Vector3 startPosition;
        public LayerMask unwalkableMask;

        public int width;
        public int height;
        public int neighbourCount = 4;
        public float edgeLength;
        public float agentHeight;
        [Range(1f,10f)]
        public float costMultiplier;

        public Graph.Region[] regions;

        public Transform target;

        public List<Graph> graphs = new List<Graph>();
        //public List<Vector3> path = new List<Vector3>();
        public Node endNode;


        public List<Node> openSet = new List<Node>();
        public List<Node> closedSet = new List<Node>();

        public bool drawGizmos;

        private Pathfinder pathfinder;
        int threadID = 0;

        void Start () {
            Graph g = new Graph(startPosition, width, height, neighbourCount, edgeLength, agentHeight, unwalkableMask, regions);
            graphs.Add(g);

            //Create Pathfinder
            //pathfinder = new Pathfinder();
            pathfinder = GetComponent<Pathfinder>();

            pathfinder.width = width;
            pathfinder.height = height;
            pathfinder.neighbourCount = neighbourCount;
            pathfinder.costMultiplier = costMultiplier;
            pathfinder.graph = graphs[0];
            pathfinder.edgeLength = edgeLength;
        }


        public void RequestPath(Agent agent, Vector3 targetPos, Action<Vector3[], bool> callback = null)
        {
            Pathfinder.RequestPath(agent.transform.position, targetPos, callback);
        }

        void OnDrawGizmos()
        {
            if (!drawGizmos || graphs.Count < 1) return;

            Graph g = graphs[0];
            for (int i = 0; i < g.width; i++)
            {
                for (int j = 0; j < g.height; j++)
                {
                    Gizmos.color = Color.yellow;
                    if (g.grid[i,j] == null) continue;
                    if (!g.grid[i,j].isWalkable)
                        Gizmos.color = Color.red;
                    Gizmos.DrawCube(g.grid[i, j].position, Vector3.one * edgeLength * .5f);
                    
                }
            }
       
        }
    }
}
