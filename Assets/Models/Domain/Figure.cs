using System.Drawing;
using UnityEngine;

namespace Domain
{
    public class Figure : IFigure
    {
        public Texture2D Texture { get; set; }
        public Bitmap ImgData { get; set; }
        public Vector4 KeyPoints { get; set; }
    }
}
