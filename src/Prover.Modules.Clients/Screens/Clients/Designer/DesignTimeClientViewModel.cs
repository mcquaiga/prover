using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Prover.GUI.Common.Screens;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Models.Clients;

namespace Prover.Modules.Clients.Screens.Clients.Designer
{
    public class DesignTimeClientViewModel
    {
        public DesignTimeClientViewModel()
        {
           // Client = JsonConvert.DeserializeObject<Core.Models.Clients.Client>(_clientJson);
           Client = new Core.Models.Clients.Client()
           {
                Name = "Adam's Energy Company",
                Address = @"4-33 Songhees Dr.
Victoria, BC
CANADA"
           };

            CreateClientItems(Client);
        }

        public Prover.Core.Models.Clients.Client Client { get; set; }

        public IEnumerable<InstrumentType> InstrumentTypes => Instruments.GetAll().ToList();

        public List<ClientItemType> ItemFileTypesList => Enum.GetValues(typeof(ClientItemType)).Cast<ClientItemType>().ToList();

        public ClientItems CurrentItemData => Client.Items.FirstOrDefault();

        private string _clientJson = "{\"Items\":[{\"ClientId\":\"c7915cc2-2583-4389-ab2b-5d2430a324ed\",\"ItemFileTypeString\":\"PreTest\",\"ClientItemFileType\":0,\"InstrumentTypeString\":\"3\",\"InstrumentData\":\"{\\\"0\\\":\\\"0\\\"}\",\"Items\":[{\"RawValue\":\"0\",\"Metadata\":{\"Number\":0,\"Code\":\"COR_VOL\",\"ShortDescription\":\"Corrected Vol\",\"LongDescription\":\"Corrected Volume\",\"IsAlarm\":false,\"IsPressure\":false,\"IsPressureTest\":false,\"IsTemperature\":false,\"IsTemperatureTest\":false,\"IsVolume\":true,\"IsVolumeTest\":true,\"IsSuperFactor\":false,\"ItemDescriptions\":[]},\"NumericValue\":0.0,\"Description\":\"0\"}],\"InstrumentType\":{\"AccessCode\":3,\"Name\":\"Mini-AT\",\"Id\":3,\"ItemFilePath\":\"MiniATItems.xml\"},\"Id\":\"bfb67258-9970-4147-8f48-97a0cfb3b460\"},{\"ClientId\":\"c7915cc2-2583-4389-ab2b-5d2430a324ed\",\"ItemFileTypeString\":\"PreTest\",\"ClientItemFileType\":0,\"InstrumentTypeString\":\"4\",\"InstrumentData\":\"{\\\"2\\\":\\\"0\\\",\\\"0\\\":\\\"0\\\"}\",\"Items\":[{\"RawValue\":\"0\",\"Metadata\":{\"Number\":2,\"Code\":\"UNCOR_VOL\",\"ShortDescription\":\"UnCor Vol\",\"LongDescription\":\"Uncorrected Volume\",\"IsAlarm\":false,\"IsPressure\":false,\"IsPressureTest\":false,\"IsTemperature\":false,\"IsTemperatureTest\":false,\"IsVolume\":true,\"IsVolumeTest\":true,\"IsSuperFactor\":false,\"ItemDescriptions\":[]},\"NumericValue\":0.0,\"Description\":\"0\"},{\"RawValue\":\"0\",\"Metadata\":{\"Number\":0,\"Code\":\"COR_VOL\",\"ShortDescription\":\"Corrected Vol\",\"LongDescription\":\"Corrected Volume\",\"IsAlarm\":false,\"IsPressure\":false,\"IsPressureTest\":false,\"IsTemperature\":false,\"IsTemperatureTest\":false,\"IsVolume\":true,\"IsVolumeTest\":true,\"IsSuperFactor\":false,\"ItemDescriptions\":[]},\"NumericValue\":0.0,\"Description\":\"0\"}],\"InstrumentType\":{\"AccessCode\":4,\"Name\":\"Mini-Max\",\"Id\":4,\"ItemFilePath\":\"MiniMaxItems.xml\"},\"Id\":\"875a6cfb-2a57-43a3-8dd8-aa89f0e13aee\"},{\"ClientId\":\"c7915cc2-2583-4389-ab2b-5d2430a324ed\",\"ItemFileTypeString\":\"Verify\",\"ClientItemFileType\":2,\"InstrumentTypeString\":\"3\",\"InstrumentData\":\"{\\\"2\\\":\\\"0\\\",\\\"0\\\":\\\"0\\\",\\\"10\\\":\\\"100\\\"}\",\"Items\":[{\"RawValue\":\"0\",\"Metadata\":{\"Number\":2,\"Code\":\"UNCOR_VOL\",\"ShortDescription\":\"UnCor Vol\",\"LongDescription\":\"Uncorrected Volume\",\"IsAlarm\":false,\"IsPressure\":false,\"IsPressureTest\":false,\"IsTemperature\":false,\"IsTemperatureTest\":false,\"IsVolume\":true,\"IsVolumeTest\":true,\"IsSuperFactor\":false,\"ItemDescriptions\":[]},\"NumericValue\":0.0,\"Description\":\"0\"},{\"RawValue\":\"0\",\"Metadata\":{\"Number\":0,\"Code\":\"COR_VOL\",\"ShortDescription\":\"Corrected Vol\",\"LongDescription\":\"Corrected Volume\",\"IsAlarm\":false,\"IsPressure\":false,\"IsPressureTest\":false,\"IsTemperature\":false,\"IsTemperatureTest\":false,\"IsVolume\":true,\"IsVolumeTest\":true,\"IsSuperFactor\":false,\"ItemDescriptions\":[]},\"NumericValue\":0.0,\"Description\":\"0\"},{\"RawValue\":\"100\",\"Metadata\":{\"Number\":10,\"Code\":\"\",\"ShortDescription\":\"Press High A\",\"LongDescription\":\"Pressure High Alarm\",\"IsAlarm\":true,\"IsPressure\":false,\"IsPressureTest\":false,\"IsTemperature\":false,\"IsTemperatureTest\":false,\"IsVolume\":false,\"IsVolumeTest\":false,\"IsSuperFactor\":false,\"ItemDescriptions\":[]},\"NumericValue\":100.0,\"Description\":\"100\"}],\"InstrumentType\":{\"AccessCode\":3,\"Name\":\"Mini-AT\",\"Id\":3,\"ItemFilePath\":\"MiniATItems.xml\"},\"Id\":\"4aeb6721-f7e9-405f-a94c-71566d46a3b8\"},{\"ClientId\":\"c7915cc2-2583-4389-ab2b-5d2430a324ed\",\"ItemFileTypeString\":\"Verify\",\"ClientItemFileType\":2,\"InstrumentTypeString\":\"4\",\"InstrumentData\":\"{\\\"2\\\":\\\"0\\\",\\\"0\\\":\\\"0\\\",\\\"10\\\":\\\"100\\\",\\\"13\\\":\\\"60\\\"}\",\"Items\":[{\"RawValue\":\"0\",\"Metadata\":{\"Number\":2,\"Code\":\"UNCOR_VOL\",\"ShortDescription\":\"UnCor Vol\",\"LongDescription\":\"Uncorrected Volume\",\"IsAlarm\":false,\"IsPressure\":false,\"IsPressureTest\":false,\"IsTemperature\":false,\"IsTemperatureTest\":false,\"IsVolume\":true,\"IsVolumeTest\":true,\"IsSuperFactor\":false,\"ItemDescriptions\":[]},\"NumericValue\":0.0,\"Description\":\"0\"},{\"RawValue\":\"0\",\"Metadata\":{\"Number\":0,\"Code\":\"COR_VOL\",\"ShortDescription\":\"Corrected Vol\",\"LongDescription\":\"Corrected Volume\",\"IsAlarm\":false,\"IsPressure\":false,\"IsPressureTest\":false,\"IsTemperature\":false,\"IsTemperatureTest\":false,\"IsVolume\":true,\"IsVolumeTest\":true,\"IsSuperFactor\":false,\"ItemDescriptions\":[]},\"NumericValue\":0.0,\"Description\":\"0\"},{\"RawValue\":\"100\",\"Metadata\":{\"Number\":10,\"Code\":\"\",\"ShortDescription\":\"Press High A\",\"LongDescription\":\"Pressure High Alarm\",\"IsAlarm\":true,\"IsPressure\":false,\"IsPressureTest\":false,\"IsTemperature\":false,\"IsTemperatureTest\":false,\"IsVolume\":false,\"IsVolumeTest\":false,\"IsSuperFactor\":false,\"ItemDescriptions\":[]},\"NumericValue\":100.0,\"Description\":\"100\"},{\"RawValue\":\"60\",\"Metadata\":{\"Number\":13,\"Code\":\"BASE_PRESS\",\"ShortDescription\":\"Base Press\",\"LongDescription\":\"Base Pressure\",\"IsAlarm\":false,\"IsPressure\":true,\"IsPressureTest\":false,\"IsTemperature\":false,\"IsTemperatureTest\":false,\"IsVolume\":false,\"IsVolumeTest\":false,\"IsSuperFactor\":false,\"ItemDescriptions\":[]},\"NumericValue\":60.0,\"Description\":\"60\"}],\"InstrumentType\":{\"AccessCode\":4,\"Name\":\"Mini-Max\",\"Id\":4,\"ItemFilePath\":\"MiniMaxItems.xml\"},\"Id\":\"0579108c-8c96-4add-88cc-cbeb0c996aae\"},{\"ClientId\":\"c7915cc2-2583-4389-ab2b-5d2430a324ed\",\"ItemFileTypeString\":\"Reset\",\"ClientItemFileType\":1,\"InstrumentTypeString\":\"3\",\"InstrumentData\":\"{}\",\"Items\":[],\"InstrumentType\":{\"AccessCode\":3,\"Name\":\"Mini-AT\",\"Id\":3,\"ItemFilePath\":\"MiniATItems.xml\"},\"Id\":\"39ace22d-4947-4054-88d7-9cac1dfa8d38\"},{\"ClientId\":\"c7915cc2-2583-4389-ab2b-5d2430a324ed\",\"ItemFileTypeString\":\"Reset\",\"ClientItemFileType\":1,\"InstrumentTypeString\":\"4\",\"InstrumentData\":\"{\\\"0\\\":\\\"0\\\",\\\"2\\\":\\\"0\\\"}\",\"Items\":[{\"RawValue\":\"0\",\"Metadata\":{\"Number\":0,\"Code\":\"COR_VOL\",\"ShortDescription\":\"Corrected Vol\",\"LongDescription\":\"Corrected Volume\",\"IsAlarm\":false,\"IsPressure\":false,\"IsPressureTest\":false,\"IsTemperature\":false,\"IsTemperatureTest\":false,\"IsVolume\":true,\"IsVolumeTest\":true,\"IsSuperFactor\":false,\"ItemDescriptions\":[]},\"NumericValue\":0.0,\"Description\":\"0\"},{\"RawValue\":\"0\",\"Metadata\":{\"Number\":2,\"Code\":\"UNCOR_VOL\",\"ShortDescription\":\"UnCor Vol\",\"LongDescription\":\"Uncorrected Volume\",\"IsAlarm\":false,\"IsPressure\":false,\"IsPressureTest\":false,\"IsTemperature\":false,\"IsTemperatureTest\":false,\"IsVolume\":true,\"IsVolumeTest\":true,\"IsSuperFactor\":false,\"ItemDescriptions\":[]},\"NumericValue\":0.0,\"Description\":\"0\"}],\"InstrumentType\":{\"AccessCode\":4,\"Name\":\"Mini-Max\",\"Id\":4,\"ItemFilePath\":\"MiniMaxItems.xml\"},\"Id\":\"e84f21ba-9416-4f1a-a24f-fc86d883728e\"}],\"Name\":\"Adam\",\"Address\":\"932 Johnson St\",\"CreatedDateTime\":\"2017-02-15T17:10:31.577\",\"ArchivedDateTime\":null,\"Id\":\"c7915cc2-2583-4389-ab2b-5d2430a324ed\"}";


        private void CreateClientItems(Core.Models.Clients.Client client)
        {
            var clientItems = new ClientItems()
            {
                Client = client,
                InstrumentType = Instruments.MiniAt,
                ItemFileType = ClientItemType.Reset,
                InstrumentData = @"{""0"":""0"", ""1"":""0""}"
            };

            Client.Items.Add(clientItems);
        }
    }
}
