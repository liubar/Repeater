using Domain;

namespace App
{
    public interface IFigureManager
    {
        IRecognitionManager RecognitionManager { get; }
        IFigureComparision FigureComparision { get; }

        IFigure GetRandomFigure();
        bool Compare(IBoard board, IFigure figure);
    }
}
