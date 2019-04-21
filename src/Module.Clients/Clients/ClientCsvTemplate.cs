using Core.Domain;
using Core.Extensions;
using Devices.Core.EvcDevices;
using Devices.Core.Interfaces;
using System;

namespace Module.Clients.Clients
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
            ClientId = client.Id;
        }

        #endregion

        #region Properties

        public virtual Client Client { get; set; }

        public Guid ClientId { get; set; }

        public EvcCorrectorType? CorrectorType { get; set; }

        public string CorrectorTypeString
        {
            get => CorrectorType.ToString();
            private set => CorrectorType = value?.ParseEnum<EvcCorrectorType?>();
        }

        public string CsvTemplate { get; set; }

        public DriveTypeDescripter? DriveType { get; set; }

        public string DriveTypeString
        {
            get => DriveType.ToString();
            private set => DriveType = value?.ParseEnum<DriveTypeDescripter?>();
        }

        public IEvcDeviceType InstrumentType { get; set; }

        public VerificationTypeEnum? VerificationType { get; set; }

        public string VerificationTypeString
        {
            get => VerificationType.ToString();
            private set => VerificationType = value?.ParseEnum<VerificationTypeEnum?>();
        }

        #endregion
    }
}