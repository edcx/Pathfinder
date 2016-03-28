using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Pathfinder.Scripts
{
    public class PathManager : MonoBehaviour
    {

        struct PathRequest
        {
            public Vector3 pathStart;
            public Vector3 pathEnd;
            public Action<Vector3[], bool> callback;

            public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
            {
                pathStart = _start;
                pathEnd = _end;
                callback = _callback;
            }
        }

        Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
        private PathRequest currentPathRequest;

        private bool isProcessingPath;

        private Pathfinder pathfinder;

        private static PathManager instance;

        public PathManager()
        {
            instance = this;

        }

        public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
        {
            PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
            instance.pathRequestQueue.Enqueue(newRequest);
            instance.TryProcessNext();
        }

        public static int ReturnQueuedRequestCount()
        {
            return instance.pathRequestQueue.Count;
        }
        void TryProcessNext()
        {
            if (!isProcessingPath && pathRequestQueue.Count > 0)
            {
                currentPathRequest = pathRequestQueue.Dequeue();
                isProcessingPath = true;
                pathfinder.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
                //IEnumerator e = FindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
                //while (e.MoveNext()){ }

            }
        }
        public void FinishedProcessingPath(Vector3[] path, bool success)
        {
            currentPathRequest.callback(path, success);
            isProcessingPath = false;
            TryProcessNext();
        }

        void Start () {

            //Create Pathfinder
            //pathfinder = new Pathfinder();
            pathfinder = GetComponent<Pathfinder>();
        }

        
    }
}
