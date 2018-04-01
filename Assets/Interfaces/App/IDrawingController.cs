using Domain;
using UnityEngine;

namespace App
{
    public interface IDrawingController
    {
        ParticleSystem TrailEffect { get; }
        LineRenderer Line { get; }
        IBoard Board { get; }
        bool DrawingIsOver { get; }

        void Update();
        void Clear();
    }
}
