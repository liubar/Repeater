namespace Domain
{
    public interface IRound
    {
        float Time { get; }
        int Number { get; }
        Complexity ComplexityLevel { get; }
    }
}
