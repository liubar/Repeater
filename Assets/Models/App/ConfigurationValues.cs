namespace App
{
    public static class ConfigurationValues
    {
        // The size of the template image
        public const int ImageSize = 64;
        // Inner indent of a figure
        public const int PaddingFigure = 10;

        // Initial playing time
        public static float StartTime = 15;
        // Accuracy of pattern matching
        public static float SimilarityThreshold = 0.9f;
        public static string TemplateFolder = "/Figures";
    }
}
