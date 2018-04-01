using System.Drawing;
using Boo.Lang;
using Domain;
using UnityEngine;

namespace App
{
    public class RadialAlgFigureComparision : IFigureComparision
    {
        private Function<int, int> InvertIndex = i => (ConfigurationValues.ImageSize - 1 - i);
        private float _similaryThreshold;
        private float _confSimilarityThreshold;

        /// <summary>
        ///     Ctor
        /// </summary>
        public RadialAlgFigureComparision()
        {
            UpdateSimilaryThreshold();
        }

        /// <summary>
        ///     Compares figures
        /// </summary>
        /// <param name="x">first figure</param>
        /// <param name="y">second figure</param>
        /// <returns>Returns 0 if the figures are equal, Otherwise it will return -1</returns>
        public int Compare(IFigure x, IFigure y)
        {
            // Updating recognition accuracy when changing
            if (_confSimilarityThreshold != ConfigurationValues.SimilarityThreshold) UpdateSimilaryThreshold();

            // Find key points
            if (x.KeyPoints == default(Vector4)) ParseKeyPoints(x);
            if (y.KeyPoints == default(Vector4)) ParseKeyPoints(y);

            return IsInTheRange(x.KeyPoints, y.KeyPoints) ? 0 : -1;
        }

        /// <summary>
        ///     Parse figure and set key points
        /// </summary>
        /// <param name="figure">Figure for parsing</param>
        private void ParseKeyPoints(IFigure figure)
        {
            var original = figure.ImgData;
            //values[0, i] = 0;  // a
            //values[1, i] = 0;  // r
            //values[2, i] = 0;  // g
            //values[3, i] = 0;  // b
            double[,] values = new Bitmap(original).To2DimDoubleArray();

            // use radial algoritm
            var first = IndexOfMainDiagonal(values, true);
            var last = IndexOfMainDiagonal(values, false);

            var firstSD = IndexOfSecondaryDiagonal(values, true);
            var lastSD = IndexOfSecondaryDiagonal(values, false);

            figure.KeyPoints = new Vector4(first, last, firstSD, lastSD);
        }

        /// <summary>
        ///     Checks the correspondence of the values within a given range
        /// </summary>
        /// <param name="fValue">Key points</param>
        /// <param name="sValue">Key points</param>
        /// <returns></returns>
        private bool IsInTheRange(Vector4 fValue, Vector4 sValue)
        {
            return Mathf.Abs(fValue.x - sValue.x) <= _similaryThreshold &&
                   Mathf.Abs(fValue.y - sValue.y) <= _similaryThreshold &&
                   Mathf.Abs(fValue.z - sValue.z) <= _similaryThreshold &&
                   Mathf.Abs(fValue.w - sValue.w) <= _similaryThreshold;
        }

        #region Search Image keys

        /// <summary>
        ///     Search at main diagonal
        ///         1 0 0
        ///         0 1 0
        ///         0 0 1
        /// </summary>
        /// <param name="array">Picture in double drray</param>
        /// <param name="ASC">
        ///     If ASC == true then search from LEFT to RIGHT along the main diagonal
        ///     If ASC == false then Search from RIGHT to LEFT along the main diagonal
        /// </param>
        /// <returns></returns>
        private int IndexOfMainDiagonal(double[,] array, bool ASC)
        {
            var result = 0;

            for (int i = 0; i < ConfigurationValues.ImageSize; i++)
            {
                int index;
                if (ASC)
                    index = (i * ConfigurationValues.ImageSize) + i;
                else
                    index = InvertIndex(i) * ConfigurationValues.ImageSize + InvertIndex(i);

                if (array[1, index] < 240 || array[2, index] < 240 || array[3, index] < 240)
                {
                    break;
                }

                result++;
            }

            return ASC ? result : ConfigurationValues.ImageSize - result;
        }

        /// <summary>
        ///     Search at secondary diagonal
        ///         0 0 1
        ///         0 1 0
        ///         1 0 0
        /// </summary>
        /// <param name="array">Picture in double drray</param>
        /// <param name="ASC">
        ///     If ASC == true then a search from LEFT to RIGHT by an auxiliary diagonal
        ///     If ASC == false then search from the RIGHT to the LEFT on the side diagonal
        /// </param>
        /// <returns></returns>
        private int IndexOfSecondaryDiagonal(double[,] array, bool ASC)
        {
            var result = 0;

            for (int i = ConfigurationValues.ImageSize - 1; i > 0; i--)
            {
                int index;
                if (ASC)
                    index = i * ConfigurationValues.ImageSize + ConfigurationValues.ImageSize - i - 1;
                else
                    index = InvertIndex(i) * ConfigurationValues.ImageSize + ConfigurationValues.ImageSize - InvertIndex(i) - 1;

                if (array[1, index] < 240 || array[2, index] < 240 || array[3, index] < 240)
                {
                    break;
                }

                result++;
            }

            return ASC ? result : ConfigurationValues.ImageSize - result;
        }

        #endregion

        /// <summary>
        ///     Update Similary Threshold
        /// </summary>
        private void UpdateSimilaryThreshold()
        {
            _confSimilarityThreshold = ConfigurationValues.SimilarityThreshold;
            _similaryThreshold = ConfigurationValues.ImageSize - ConfigurationValues.ImageSize * ConfigurationValues.SimilarityThreshold;
        }
    }
}
