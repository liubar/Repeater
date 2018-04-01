using Domain;

namespace App
{
    public interface IEasel
    {
        IFigure PatternFigure { get; set; }
        IBoard Board { get; }
    }
}
