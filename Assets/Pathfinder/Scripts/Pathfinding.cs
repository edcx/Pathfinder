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
        [Range(0f,1f)]
        public float costModifier;


        public Transform target;

        public List<Graph> graphs = new List<Graph>();
        public List<Vector3> path = new List<Vector3>();
        public Node endNode;


        public List<Node> openSet = new List<Node>();
        public List<Node> closedSet = new List<Node>();

        public bool UseMultiThread = false;
        public bool drawGizmos;


        PathfindingThread pfThread;
        int threadID = 0;
        private GameObject graphContainer;
        void Start () {
            Graph g = new Graph(startPosition, width, height, neighbourCount, edgeLength, unwalkableMask);
            graphs.Add(g);

            graphContainer = new GameObject("Graph");
            for (int i = 0; i < graphs[0].grid.GetLength(0); i++)
            {
                for (int j = 0; j < graphs[0].grid.GetLength(1); j++)
                {
                    GameObject node = new GameObject(i + "x" + j);
                    node.transform.parent = graphContainer.transform;
                    node.transform.position = graphs[0].grid[i, j].position;
                    BoxCollider collider = node.AddComponent<BoxCollider>();
                    collider.size = Vector3.one * edgeLength;
                    NodeDisplayer nd = node.AddComponent<NodeDisplayer>();

                }
            }

        }
	

        public void RequestPath(Agent agent, Vector3 targetPos)
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
            Profiler.BeginSample("pf");
            Pathfinder pf = new Pathfinder();
            pf.callBackListener = agent;
            pf.startPosition = agent.transform.position;
            pf.targetPosition = targetPos;
            pf.width = width;
            pf.height = height;
            pf.neighbourCount = neighbourCount;
            pf.costModifier = costModifier;
            Profiler.EndSample();
            Profiler.BeginSample("Graph Copy");
            pf.graph = graphs[0];
            Profiler.EndSample();
            pf.edgeLength = edgeLength;

            Profiler.BeginSample("Calculate");
            pf.CalculatePath();
            Profiler.EndSample();

            for (int i = 0; i < graphs[0].grid.GetLength(0); i++)
            {
                for (int j = 0; j < graphs[0].grid.GetLength(1); j++)
                {
                    Transform node = graphContainer.transform.FindChild(i + "x" + j);
                    NodeDisplayer nd = node.GetComponent<NodeDisplayer>();
                    nd.f = graphs[0].grid[i, j].f;
                    nd.g = graphs[0].grid[i, j].g;
                    nd.h = graphs[0].grid[i, j].h;
                    nd.position = graphs[0].grid[i, j].position;
                }
            }

            return;

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
