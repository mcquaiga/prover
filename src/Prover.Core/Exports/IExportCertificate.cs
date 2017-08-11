using System.Threading.Tasks;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Clients;

namespace Prover.Core.Exports
{
    public interface IExportCertificate
    {
        Task<string[]> Export(Client client, long fromCertificateNumber, long toCertificateNumber);
        Task<string> Export(Certificate certificate);
    }
}