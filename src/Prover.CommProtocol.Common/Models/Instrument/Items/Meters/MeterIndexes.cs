using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Prover.CommProtocol.Common.Models.Instrument.Items.Meters
{
    public class MeterIndexInfo
    {
        private MeterIndexInfo()
        {
        }

        private MeterIndexInfo(int id, string description, int unCorPulsesX10, int unCorPulsesX100,
            double? meterDisplacement)
        {
            Id = id;
            Description = description;
            UnCorPulsesX10 = unCorPulsesX10;
            UnCorPulsesX100 = unCorPulsesX100;
            MeterDisplacement = meterDisplacement;
        }

        public string Description { get; set; }

        public int Id { get; set; }
        public double? MeterDisplacement { get; set; }
        public int UnCorPulsesX10 { get; set; }
        public int UnCorPulsesX100 { get; set; }

        public static MeterIndexInfo Get(int meterIndexId)
        {
            var xDoc = XDocument.Load("MeterIndexes.xml");

            var indexes = from x in xDoc.Descendants("value")
                where Convert.ToInt32((string) x.Attribute("id").Value) == meterIndexId
                select new MeterIndexInfo
                {
                    Id = Convert.ToInt32(x.Attribute("id").Value),
                    Description = x.Attribute("description").Value,
                    UnCorPulsesX10 = Convert.ToInt32(x.Attribute("UnCorPulsesX10").Value),
                    UnCorPulsesX100 = Convert.ToInt32(x.Attribute("UnCorPulsesX100").Value),
                    MeterDisplacement = Convert.ToDouble(x.Attribute("MeterDisplacement").Value)
                };

            if (indexes == null || !indexes.Any())
                throw new KeyNotFoundException(
                    string.Format("Could not find a matching meter index definition for Meter Index Id: {0}",
                        meterIndexId));

            return indexes.FirstOrDefault();
        }
    }
}