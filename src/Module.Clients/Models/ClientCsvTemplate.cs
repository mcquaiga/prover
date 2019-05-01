using Core;
using Core.Domain;
using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;

namespace Module.Clients.Models
{
    public class ClientCsvTemplate : BaseEntity
    {
        #region Constructors

        public ClientCsvTemplate()
        {
        }

        public ClientCsvTemplate(Client client) : this()
        {
            Client = client;
        }

        #endregion

        #region Properties

        public virtual Client Client { get; set; }

        public EvcCorrectorType? CorrectorType { get; set; }

        public string CsvTemplate { get; set; }

        public IDeviceType DeviceType { get; set; }

        public IDriveType DriveType { get; set; }

        public VerificationType VerificationType { get; set; }

        #endregion
    }
}