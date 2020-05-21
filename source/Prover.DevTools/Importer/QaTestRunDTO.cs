using System;
using System.Collections.Generic;

namespace Prover.Modules.DevTools.Importer
{
    internal class SessionContext
    {
        internal SessionContext() => TestItems = new List<QaTestRunDTO>();

        internal List<QaTestRunDTO> TestItems { get; }
    }

    public class QaTestRunDTO
    {
        public QaTestRunDTO(Guid deviceTypeId, Dictionary<string, string> values) => Device = new DeviceDTO { DeviceTypeId = deviceTypeId, Items = values };

        public DeviceDTO Device { get; set; }

        public ICollection<TestDTO> Tests { get; set; } = new List<TestDTO>();

        public DateTime TestDateTime { get; set; }

        public DateTime? ArchivedDateTime { get; set; } = null;

        public DateTime? ExportedDateTime { get; set; } = null;

        public bool IsPassed { get; set; }

        public string JobId { get; set; }

        public string EmployeeId { get; set; }
        //public void UpdateTestPoints()
        //{
        //    TestPoints.ForEach(tp =>
        //    {
        //        tp.ItemValues = Device.DeviceType.ToItemValues(tp.Values)
        //                              .ToList();
        //    });
        //    //.SelectMany(tp => .A tp.ItemValues = v))
        //    //  .Select(v => )
        //}

        public void AddTest(Dictionary<string, string> itemValues)
        {
            Tests.Add(new TestDTO { Values = itemValues });
        }
    }

    public class TestDTO
    {
        public int TestNumber { get; set; }
        public Dictionary<string, string> Values { get; set; }
        public Dictionary<string, string> EndValues { get; set; } = new Dictionary<string, string>();
        public decimal AppliedInput { get; set; }
        public int UnCorPulses { get; set; }
        public int CorPulses { get; set; }
    }


    public class DeviceDTO
    {
        public Guid DeviceTypeId { get; set; }
        public Dictionary<string, string> Items { get; set; }
    }

    internal static class Mappers
    {
        public static Dictionary<int, Guid> DeviceTypeMappings = new Dictionary<int, Guid>
        {
                {4, Guid.Parse("f29d8c85-8183-4256-9b14-369ef440ec60")},
                {99, Guid.Parse("8b247a7e-0ed2-41d4-a6b9-45834b7f14e1")},
                {3, Guid.Parse("ced367b2-58dd-4ddd-bccf-43662f4ab36d")}
        };
    }
}