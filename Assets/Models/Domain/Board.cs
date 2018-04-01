using UnityEngine;

namespace Domain
{
    public class Board : IBoard
    {
        public Vector3 MinPosition { get; set; }
        public Vector3 MaxPosition { get; set; }

        public bool IsEmpty
        {
            get { return MinPosition == default(Vector3) || MaxPosition == default(Vector3); }
        }

        public void Clear()
        {
            MinPosition = default(Vector3);
            MaxPosition = default(Vector3);
        }
    }
}
