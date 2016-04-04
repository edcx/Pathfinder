using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Pathfinder.Scripts
{
    public class PathManager : MonoBehaviour
    {

        /// <summary>
        /// Path request data type
        /// New request should be sent to manager when searching for a path
        /// </summary>
        struct PathRequest
        {
            public Vector3 PathStart;
            public Vector3 PathEnd;
            public Action<Vector3[], bool> Callback;

            public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> callback)
            {
                PathStart = start;
                PathEnd = end;
                Callback = callback;
            }
        }

        Queue<PathRequest> _pathRequestQueue = new Queue<PathRequest>();
        private PathRequest _currentPathRequest;

        private bool _isProcessingPath;

        private Pathfinder _pathfinder;

        private static PathManager _instance;

        public PathManager()
        {
            _instance = this;

        }

        /// <summary>
        /// Creates a path request and tries to process it
        /// </summary>
        /// <param name="pathStart"></param>
        /// <param name="pathEnd"></param>
        /// <param name="callback"></param>
        public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
        {
            PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
            _instance._pathRequestQueue.Enqueue(newRequest);
            _instance.TryProcessNext();
        }

        public static int ReturnQueuedRequestCount()
        {
            return _instance._pathRequestQueue.Count;
        }
        void TryProcessNext()
        {
            if (!_isProcessingPath && _pathRequestQueue.Count > 0)
            {
                _currentPathRequest = _pathRequestQueue.Dequeue();
                _isProcessingPath = true;
                _pathfinder.StartFindPath(_currentPathRequest.PathStart, _currentPathRequest.PathEnd);
                //IEnumerator e = FindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
                //while (e.MoveNext()){ }

            }
        }

        /// <summary>
        /// Pathinding Process finished. Send callback and try next request.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="success"></param>
        public void FinishedProcessingPath(Vector3[] path, bool success)
        {
            _currentPathRequest.Callback(path, success);
            _isProcessingPath = false;
            TryProcessNext();
        }

        void Start () {

            //Create Pathfinder
            _pathfinder = GetComponent<Pathfinder>();
        }

        
    }
}
