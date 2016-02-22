using System.Collections.Generic;
using UnityEngine;

namespace Assets.Pathfinder.Scripts
{
    public class Pathfinding : MonoBehaviour {

        public Vector3 startPosition;

        public int width;
        public int height;
        public int neighbourCount = 4;
        public float edgeLength;
        [Range(0f,1f)]
        public float costModifier;

        public bool drawGizmos;

        public Transform target;
        //public Transform agent;

        public List<Graph> graphs = new List<Graph>();
        public List<Vector3> path = new List<Vector3>();
        public Node endNode;


        public List<Node> openSet = new List<Node>();
        public List<Node> closedSet = new List<Node>();

        PathfindingThread pfThread;
        int threadID = 0;
        // Use this for initialization
        void Start () {
            Graph g = new Graph(startPosition, width, height, neighbourCount, edgeLength);
            graphs.Add(g);
        
        }
	
        // Update is called once per frame
        void Update () {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //TODO: Rather than giving the actual graph data, create a clone for multiple path requests
                //graphs[0].ClearGraphParentData();

            }
            /*if (pfThread != null)
        {
            if (pfThread.Update())
            {
                // Alternative to the OnFinished callback
                path = pfThread.path;
                pfThread = null;
            }
        }*/
        }

        public void RequestPath(Agent agent, Vector3 targetPos)
        {
            pfThread = new PathfindingThread();
            pfThread.Id = threadID;
            pfThread.callBackListener = agent;
            pfThread.startPosition = agent.transform.position;
            pfThread.endPosition = targetPos;
            pfThread.width = width;
            pfThread.height = height;
            pfThread.neighbourCount = neighbourCount;
            pfThread.costModifier = costModifier;

            pfThread.graph = graphs[0].DeepCopy();

            pfThread.edgeLength = edgeLength;
            pfThread.Start();
            threadID++;
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
                    if (!g.GetNodeAtIndex(i,j).isWalkable)
                        Gizmos.color = Color.green;
                    Gizmos.DrawCube(g.GetNodeAtIndex(i, j).position, Vector3.one * .5f);
                }
            }
       
        }
    }
}
