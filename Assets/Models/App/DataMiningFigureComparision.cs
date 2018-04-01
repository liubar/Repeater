using System.Drawing;
using AForge.Imaging;
using AForge.Imaging.Filters;
using Domain;

namespace App
{
    public class DataMiningFigureComparision : IFigureComparision
    {
        ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(ConfigurationValues.SimilarityThreshold);
        ResizeBilinear filter = new ResizeBilinear(ConfigurationValues.ImageSize, ConfigurationValues.ImageSize);
        
        public int Compare(IFigure x, IFigure y)
        {
            // apply the filter
            Bitmap newImage = filter.Apply(x.ImgData);

            // Updating recognition accuracy when changing
            if (tm.SimilarityThreshold != ConfigurationValues.SimilarityThreshold)
            {
                tm = new ExhaustiveTemplateMatching(ConfigurationValues.SimilarityThreshold);
            }

            // find all matchings with specified above similarity
            TemplateMatch[] matches = tm.ProcessImage(newImage, y.ImgData);

            return matches.Length > 0 ? 0 : -1;
        }
    }
}
