using UnityEngine;

namespace Assets.Pathfinder.Scripts
{
    public interface IPathable
    {
        int HeapIndex { get; set; }

        bool IsWalkable { get; set; }
        int G { get; set; }
        int H { get; set; }
        int F { get; }
        int X { get; set; }
        int Y { get; set; }
        int Id { get; set; }
        int Penalty { get; set; }
        Vector3 Position { get; set; }
        IPathable Parent { get; set; }
        int CompareTo(IPathable other);

    }
}
