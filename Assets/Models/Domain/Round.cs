namespace Domain
{
    public class Round : IRound
    {
        public Round(int number, float time, Complexity complexityLevel)
        {
            Number = number;
            Time = time;
            ComplexityLevel = complexityLevel;
        }

        public float Time { get; private set; }
        public int Number { get; private set; }
        public Complexity ComplexityLevel { get; private set; }
    }
}
