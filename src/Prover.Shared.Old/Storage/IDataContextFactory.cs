namespace Prover.Shared.Storage
{
	public interface IDataContextFactory<T>
	{
        T Create();
	}
}
