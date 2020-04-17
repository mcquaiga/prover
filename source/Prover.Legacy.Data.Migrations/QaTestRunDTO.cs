using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Repository;

namespace Prover.Legacy.Data.Migrations
{

    public class DataTransfer
    {
        private static SessionContext _session;
        private static List<DeviceType> _devices;
        private DataTransfer()
        {
            _session = new SessionContext();   
        }

        public static async Task<QaTestRunDTO> Create(int deviceId, Dictionary<string, string> values)
        {
            if (_devices == null)
            {
                var repo = DeviceRepository.Instance;
                _devices = repo.GetAll().ToList();
            }
               

            if (Mappers.DeviceTypeMappings.ContainsKey(deviceId))
            {
                if (_session == null)
                    _session = new SessionContext();


                //_session.TestItems.Add();
            }
            return new QaTestRunDTO(Mappers.DeviceTypeMappings[deviceId], values); 
           // VerificationTestService
        }

        public static DeviceType GetDevice(int deviceId)
        {
            return _devices.FirstOrDefault(d => d.Id == Mappers.DeviceTypeMappings[deviceId]);
        }
    }

    internal class SessionContext
    {
        internal List<QaTestRunDTO> TestItems { get; }

        internal SessionContext()
        {
            TestItems = new List<QaTestRunDTO>();
        }
    }

    //public class Verification
    //{
    //    internal static IDeviceRepository _deviceRepository = DeviceRepository.Instance;
    //    internal static IVerificationTestService _testService = ;


    //    internal Verification(DeviceDTO device, Dictionary<string, string> values)
    //    {

    //    }

    //    public static QaTestRunDTO CreatDto(int deviceId, Dictionary<string, string> values)
    //    {

    //    }
    //}

    public class QaTestRunDTO
    {
        internal QaTestRunDTO(Guid deviceTypeId, Dictionary<string, string> values)
        {

        }
        
        public DeviceDTO Device { get; set; }

        public ICollection<TestDTO> Tests { get; } = new List<TestDTO>();
    
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
            Tests.Add(new TestDTO() {Values =  itemValues});
        }

    }

    public class TestDTO
    {
        public Dictionary<string, string> Values { get; set; }
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
                { 4, Guid.Parse("f29d8c85-8183-4256-9b14-369ef440ec60")}
        };
    }
}