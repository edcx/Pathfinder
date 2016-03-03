using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Pathfinder.Scripts
{
    public class Pathfinding : MonoBehaviour {

        public Vector3 startPosition;
        public LayerMask unwalkableMask;

        public int width;
        public int height;
        public int neighbourCount = 4;
        public float edgeLength;
        public float agentHeight;
        [Range(0f,1f)]
        public float costModifier;

        public Graph.Region[] regions;

        public Transform target;

        public List<Graph> graphs = new List<Graph>();
        public List<Vector3> path = new List<Vector3>();
        public Node endNode;


        public List<Node> openSet = new List<Node>();
        public List<Node> closedSet = new List<Node>();

        public bool UseMultiThread = false;
        public bool drawGizmos;

        private Pathfinder pathfinder;
        PathfindingThread pfThread;
        int threadID = 0;

        void Start () {
            Graph g = new Graph(startPosition, width, height, neighbourCount, edgeLength, agentHeight, unwalkableMask, regions);
            graphs.Add(g);

            //Create Pathfinder
            Profiler.BeginSample("pf");
            //pathfinder = new Pathfinder();
            pathfinder = GetComponent<Pathfinder>();

            pathfinder.width = width;
            pathfinder.height = height;
            pathfinder.neighbourCount = neighbourCount;
            pathfinder.costModifier = costModifier;
            Profiler.EndSample();
            Profiler.BeginSample("Graph Copy");
            pathfinder.graph = graphs[0];
            Profiler.EndSample();
            pathfinder.edgeLength = edgeLength;
        }


        public void RequestPath(Agent agent, Vector3 targetPos, Action<Vector3[], bool> callback = null)
        {
            if (UseMultiThread)
            {
                Profiler.BeginSample("ThreadCreation");
                pfThread = new PathfindingThread();
                pfThread.Id = threadID;
                pfThread.callBackListener = agent;
                pfThread.startPosition = agent.transform.position;
                pfThread.targetPosition = targetPos;
                pfThread.width = width;
                pfThread.height = height;
                pfThread.neighbourCount = neighbourCount;
                pfThread.costModifier = costModifier;
                Profiler.EndSample();
                Profiler.BeginSample("Graph Copy");
                pfThread.graph = graphs[0].DeepCopy();
                Profiler.EndSample();
                pfThread.edgeLength = edgeLength;

                Profiler.BeginSample("Thread run");
                pfThread.Start();
                Profiler.EndSample();
                threadID++;
                return;
            }
            

            Profiler.BeginSample("Calculate");
            Pathfinder.RequestPath(agent.transform.position, targetPos, callback);
            Profiler.EndSample();

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
                    if (!g.grid[i,j].isWalkable)
                        Gizmos.color = Color.green;
                    Gizmos.DrawCube(g.grid[i, j].position, Vector3.one * .5f);
                    
                }
            }
       
        }
    }
}
