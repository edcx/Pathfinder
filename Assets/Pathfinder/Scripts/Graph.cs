using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Pathfinder.Scripts
{
    public abstract class Graph : MonoBehaviour{

        [Serializable]
        public struct Region
        {
            public LayerMask regionMask;
            public int penalty;
        }

        public abstract IPathable GetNode(Vector3 worldPosition);

        public abstract List<IPathable> GetNeighbours(IPathable node);

        public abstract int NodeCount();
    }
}
