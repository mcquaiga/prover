using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Prover.CommProtocol.Common.Models
{
    public class MeterIndex
    {    
        public MeterIndex () { }
        public IEnumerable<int> Ids { get; set; } = new List<int>();
        public string Description { get; set; }
        public int UnCorPulsesX10 { get; set; }
        public int UnCorPulsesX100 { get; set; }
        public decimal? MeterDisplacement { get; set; }
    }

    public static class MeterIndexInfo
    {
        private static IEnumerable<MeterIndex> _meterIndexes;

        public static MeterIndex Get(int meterIndexId)
        {            
            if (_meterIndexes == null || !_meterIndexes.Any())
            {
                var xDoc = XDocument.Load("MeterIndexes.xml");
                var indexes = xDoc.Descendants("value")
                    .Select(x => new MeterIndex
                        {
                            Description = x.Attribute("description").Value,
                            UnCorPulsesX10 = Convert.ToInt32(x.Attribute("UnCorPulsesX10").Value),
                            UnCorPulsesX100 = Convert.ToInt32(x.Attribute("UnCorPulsesX100").Value),
                            MeterDisplacement = Convert.ToDecimal(x.Attribute("MeterDisplacement").Value),
                            Ids = (from id in x.Elements("id")
                               select int.Parse(id.Value))
                    });

                if (indexes == null || !indexes.Any())
                    throw new KeyNotFoundException(
                        string.Format("Could not find a matching meter index definition for Meter Index Id: {0}",
                            meterIndexId));

                _meterIndexes = indexes;
            }
            
            return _meterIndexes
                    .FirstOrDefault(x => x.Ids.Contains(meterIndexId));
        }
    }
}