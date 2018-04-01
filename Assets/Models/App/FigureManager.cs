using Domain;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEngine;

namespace App
{
    public class FigureManager : IFigureManager
    {
        private List<IFigure> _figureList = new List<IFigure>();

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="figureComparision">IFigureComparision for comparing figures</param>
        /// <param name="recognitionManager">IRecognitionManager for parse figure</param>
        public FigureManager(IFigureComparision figureComparision, IRecognitionManager recognitionManager)
        {
            Utils.ThrowIfNull(figureComparision, "IFigureComparision not set to instance of FigureManager");
            Utils.ThrowIfNull(recognitionManager, "IRecognitionManager not set to instance of FigureManager");

            FigureComparision = figureComparision;
            RecognitionManager = recognitionManager;
            LoadFigures();
        }

        public IRecognitionManager RecognitionManager { get; private set; }
        public IFigureComparision FigureComparision { get; private set; }

        /// <summary>
        ///     Parse custom figure of board and compare with figure
        /// </summary>
        /// <param name="board">Main board</param>
        /// <param name="figure">comparable figure</param>
        /// <returns>Returns 0 if the figures are equal</returns>
        public bool Compare(IBoard board, IFigure figure)
        {
            var bFig = RecognitionManager.ParseFigure(board);

            if (bFig == null)
            {
                return false;
            }

            return FigureComparision.Compare(bFig, figure) == 0;
        }

        /// <summary>
        ///     Get random template figure
        /// </summary>
        /// <returns>Template figure</returns>
        public IFigure GetRandomFigure()
        {
            return _figureList[Random.Range(0, _figureList.Count)];
        }

        /// <summary>
        ///     Loading all figure templates in List
        /// </summary>
        private void LoadFigures()
        {
            var files = Directory.GetFiles(Application.streamingAssetsPath + ConfigurationValues.TemplateFolder, "*.jpg");

            foreach (var file in files)
            {
                var figure = new Figure();
                var tex = new Texture2D(ConfigurationValues.ImageSize, ConfigurationValues.ImageSize, TextureFormat.ARGB32, false);
                var bytes = File.ReadAllBytes(file);

                tex.LoadImage(bytes);
                figure.ImgData = (Bitmap)Bitmap.FromFile(file);
                figure.Texture = tex;

                _figureList.Add(figure);
            }
        }
    }
}
