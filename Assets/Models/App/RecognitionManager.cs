using System.Drawing;
using System.IO;
using Boo.Lang;
using Domain;
using UnityEngine;

namespace App
{
    public class RecognitionManager : IRecognitionManager
    {
        private int _fileNumber = 0;
        private float _templateFigureSize = 0;
        private ParticleSystem _trailEffect;

        /// <summary>
        ///     Ctor
        /// </summary>
        public RecognitionManager(ParticleSystem trailEffect)
        {
            Utils.ThrowIfNull(trailEffect, "trailEffect not set to instance of RecognitionManager");

            _trailEffect = trailEffect;
            _templateFigureSize = ConfigurationValues.ImageSize - ConfigurationValues.PaddingFigure * 2;
        }

        /// <summary>
        ///     Parse custom figure of board
        /// </summary>
        /// <param name="board">Main board</param>
        /// <returns>The drawn figure</returns>
        public IFigure ParseFigure(IBoard board)
        {
            IFigure result = new Figure();
            result.Texture = ParseBoard(board);

            if (result.Texture == null)
            {
                return null;
            }

            var tempFile = WriteToTempFile(result.Texture);
            result.ImgData = (Bitmap)Bitmap.FromFile(tempFile);

            return result;
        }

        /// <summary>
        ///     Cuts the drawn texture. It takes into account all offsets.
        /// </summary>
        /// <param name="board">Main board</param>
        /// <returns>The drawn texture</returns>
        private Texture2D ParseBoard(IBoard board)
        {
            Function<float, float> DefineMergin = f => (f / _templateFigureSize) * (float)ConfigurationValues.PaddingFigure;

            var minVector = board.MinPosition;
            var maxVector = board.MaxPosition;

            #region Determine the size of the texture and indents, taking into account the further scaling

            // Define width and height of the texture WITHOUT margins
            var widthTexMinus = (int)maxVector.x - (int)minVector.x;
            var heigthTexMinus = (int)maxVector.y - (int)minVector.y;

            // Definition of horizontal and vertical margins
            var _marginHor = DefineMergin(widthTexMinus);
            var _marginVer = DefineMergin(heigthTexMinus);

            
            // Define width and height of the texture WITH margins
            var widthTex = (int) maxVector.x - (int) minVector.x + 2 * _marginHor;
            var heightTex = (int) maxVector.y - (int) minVector.y + 2 * _marginVer;
           
            // Checking for the texture beyond the screen or empty size
            if (widthTexMinus == 0 || 
                heigthTexMinus == 0 || 
                minVector.x < 0 ||
                minVector.y < 0 ||
                maxVector.x > Screen.width ||
                maxVector.y > Screen.height)
            {
                return null;
            }

            // Crop the image if the indents do not fit on the screen
            if (minVector.x - _marginHor < 0 )
            {
                widthTex -= Mathf.Abs(minVector.x - _marginHor);
            }
            if (maxVector.x + _marginHor > Screen.width)
            {
                widthTex -= (maxVector.x + _marginHor) - Screen.width;
            }
            if (minVector.y - _marginVer < 0 )
            {
                heightTex -= Mathf.Abs(minVector.y - _marginVer);
            }
            if (maxVector.y + _marginVer > Screen.height)
            {
                heightTex -= (maxVector.y + _marginVer) - Screen.height;
            }

            #endregion

            // Disable Trail Effect
            _trailEffect.gameObject.SetActive(false);

            #region Screenshot

            RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
            rt.useMipMap = false;
            rt.antiAliasing = 1;

            RenderTexture.active = rt;
            Camera.main.targetTexture = rt;
            GameObject.Destroy(rt);

            var shot = new Texture2D((int)widthTex, (int)heightTex, TextureFormat.ARGB32, false);
            Camera.main.Render();

            // Find start position for screenshot
            var startX = minVector.x - _marginHor;
            var startY = Screen.height - (maxVector.y + _marginVer);

            if (startX < 0) startX = 0;
            if (startY < 0) startY = 0;

            //var rec = new Rect(startX, startY, widthTex, heightTex);
            var rec = new Rect(startX, startY, widthTex, heightTex);
            shot.ReadPixels(rec, 0, 0, false);
            shot.Apply();

            #endregion

            shot = Resize(shot, ConfigurationValues.ImageSize, ConfigurationValues.ImageSize);
            Camera.main.targetTexture = null;
            RenderTexture.active = null;

            // Enable Trail Effect
            _trailEffect.gameObject.SetActive(true);

            return shot;
        }

        /// <summary>
        ///     Write texture to file
        /// </summary>
        /// <param name="texture">Recordable texture</param>
        /// <returns>File path</returns>
        private string WriteToTempFile(Texture2D texture)
        {
            _fileNumber++;
            if (_fileNumber > 50) _fileNumber = 0;

            var pngOutPath = Application.streamingAssetsPath + "/Temp/temp" + _fileNumber + ".jpg";

            using (FileStream fs = new FileStream(pngOutPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var buffer = texture.EncodeToJPG();
                fs.Write(buffer, 0, buffer.Length);
            }

            return pngOutPath;
        }

        /// <summary>
        ///     Resize texture
        /// </summary>
        /// <param name="source">Texture source</param>
        /// <param name="newWidth">New width</param>
        /// <param name="newHeight">New height</param>
        /// <returns>Modified texture</returns>
        private Texture2D Resize(Texture2D source, int newWidth, int newHeight)
        {
            source.filterMode = FilterMode.Point;
            RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
            rt.filterMode = FilterMode.Point;
            RenderTexture.active = rt;

            UnityEngine.Graphics.Blit(source, rt);
            var nTex = new Texture2D(newWidth, newHeight);
            nTex.ReadPixels(new Rect(0, 0, newWidth, newWidth), 0, 0);
            nTex.Apply();
            RenderTexture.active = null;

            return nTex;
        }
    }
}
