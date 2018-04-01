using Domain;

namespace App
{
    public interface IRecognitionManager
    {
        IFigure ParseFigure(IBoard board);
    }
}
