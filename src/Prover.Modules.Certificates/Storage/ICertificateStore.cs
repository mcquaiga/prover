using Prover.Core.Storage;

namespace Prover.Modules.Certificates.Storage
{
    public interface ICertificateStore<T> : IProverStore<T>
        where T : class
    {
    }
}