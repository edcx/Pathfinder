using UnityEngine;

namespace Assets.Pathfinder.Scripts
{
    public interface IPathable
    {
        int HeapIndex { get; set; }

        bool IsWalkable { get; set; }
        int g { get; set; }
        int h { get; set; }
        int x { get; set; }
        int y { get; set; }
        int id { get; set; }
        int penalty { get; set; }
        Vector3 position { get; set; }
        IPathable parent { get; set; }
        int CompareTo(IPathable other);

        int f { get; }
    }
}
