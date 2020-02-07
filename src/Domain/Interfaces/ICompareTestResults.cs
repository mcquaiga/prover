namespace Domain.Interfaces
{
    public interface ICompareTestResults<T> where T : struct
    {
        T Actual { get; }
        T Expected { get; }
        T Variance { get; }
    }
}