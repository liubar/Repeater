using System.Drawing;
using UnityEngine;

namespace Domain
{
    public interface IFigure
    {
        Texture2D Texture { get; set; }
        Bitmap ImgData { get; set; }
        Vector4 KeyPoints { get; set; }
    }
}