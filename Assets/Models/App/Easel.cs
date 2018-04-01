using Domain;

namespace App
{
    public class Easel : IEasel
    {
        private IBoard _board;

        public Easel(IBoard board)
        {
            _board = board;
        }

        public Easel()
        {
            _board = new Board();
        }

        public IBoard Board
        {
            get { return _board; }
        }

        public IFigure PatternFigure { get; set; }
    }
}
