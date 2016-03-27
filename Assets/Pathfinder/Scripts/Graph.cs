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

        public abstract Node GetNode(Vector3 worldPosition);

        public abstract List<Node> GetNeighbours(Node node);

        public abstract int NodeCount();
    }
}
