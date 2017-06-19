using System.Threading.Tasks;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Clients;

namespace Prover.Core.Exports
{
    public interface IExportCertificate
    {
        Task<bool> Export(Client client, long fromCertificateNumber, long toCertificateNumber);
        Task<bool> Export(Certificate certificate);
    }
}