using UnityEngine;

namespace Domain
{
    public interface IBoard
    {
        Vector3 MinPosition { get; set; }
        Vector3 MaxPosition { get; set; }
        bool IsEmpty { get; }
        void Clear();
    }
}
