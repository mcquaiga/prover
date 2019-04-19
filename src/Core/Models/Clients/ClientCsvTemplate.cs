using System;

namespace Prover.Core.Models.Clients
{
    public class ClientCsvTemplate : EntityWithId
    {
        public ClientCsvTemplate()
        {           
        }

        public ClientCsvTemplate(Client client) : this()
        {
            Client = client;
            ClientId = client.Id;
        }

        [Required]
        public virtual Client Client { get; set; }

        public Guid ClientId { get; set; }

        [NotMapped]
        public VerificationTypeEnum? VerificationType { get; set; }

        [Column("VerificationType")]
        public string VerificationTypeString
        {
            get => VerificationType.ToString();
            private set => VerificationType = value?.ParseEnum<VerificationTypeEnum?>();
        }

        [NotMapped]
        public InstrumentType InstrumentType { get; set; }

        [Column("InstrumentType")]
        public string InstrumentTypeString
        {
            get => InstrumentType.Name;
            private set => InstrumentType = HoneywellInstrumentTypes.GetByName(value);
        }

        [NotMapped]
        public EvcCorrectorType? CorrectorType { get; set; }

        [Column("CorrectorType")]
        public string CorrectorTypeString
        {
            get => CorrectorType.ToString();
            private set => CorrectorType = value?.ParseEnum<EvcCorrectorType?>();
        }

        [NotMapped]
        public DriveTypeDescripter? DriveType { get; set; }

        [Column("DriveType")]
        public string DriveTypeString
        {
            get => DriveType.ToString();
            private set => DriveType = value?.ParseEnum<DriveTypeDescripter?>();
        }

        public string CsvTemplate { get; set; }
    }
}