using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using Newtonsoft.Json;
using Prover.Application.Extensions;
using Prover.Application.Interfaces;
using Prover.Application.Mappers;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.ViewModels;
using Prover.Shared.Storage.Interfaces;
using Prover.Storage.LiteDb;

namespace Prover.Legacy.Data.Migrations
{

    public class DataTransfer
    {
        private static SessionContext _session;
        private static List<DeviceType> _devices;
        static DataTransfer()
        {
            _session = new SessionContext();
            _devices = Devices.Core.Repository.DeviceRepository.Instance.GetAll().ToList();
        }

        public static Guid GetDeviceTypeId(int deviceId)
        {
            if (_devices == null)
            {
                var repo = DeviceRepository.Instance;
                _devices = repo.GetAll().ToList();
            }

            return Mappers.DeviceTypeMappings[deviceId];
        }

        public static async Task ImportTests(IVerificationTestService testService, string folderPath)
        {
            
            if (!Directory.Exists(folderPath)) throw new DirectoryNotFoundException($"{folderPath}");

            var testModels = new List<EvcVerificationTest>();

            foreach (var file in Directory.EnumerateFiles(folderPath))
            {
                using (var reader = new StreamReader(file))
                {
                    var json = await reader.ReadToEndAsync();

                    if (!string.IsNullOrEmpty(json))
                    {
                        var qaTest = JsonConvert.DeserializeObject<QaTestRunDTO>(json);
                        var deviceType = DeviceRepository.Instance.GetById(qaTest.Device.DeviceTypeId);
                        var device = deviceType.CreateInstance(qaTest.Device.Items);
                        var viewModel = testService.NewVerification(device);

                        viewModel.VerificationTests.OfType<VerificationTestPointViewModel>()
                                 .ForEach(vt =>
                                 {
                                     try
                                     {
                                         var qaVt = qaTest.Tests.FirstOrDefault(qa => qa.TestNumber == vt.TestNumber);

                                         var values = deviceType.ToItemValues(qaVt?.Values)
                                                                .ToList();
                                         vt.UpdateValues(values, device);

                                         if (vt.Volume != null)
                                         {
                                             vt.UpdateValues(values,
                                                     deviceType.ToItemValues(qaVt?.EndValues)
                                                               .ToList(),
                                                     device);
                                         }
                                     }
                                     catch (Exception ex)
                                     {
                                         Debug.WriteLine(ex);
                                         Console.WriteLine(ex);
                                     }
                                     
                                 });
                        //qaTest.Tests.ForEach(t =>
                        //{
                        //    viewModel.SetItems<>();
                        //})
                        //viewModel.SetItems<>();
                        //var model = new EvcVerificationTest(device);
                        //testModels.Add(model);
                        await testService.AddOrUpdate(viewModel);
                    }
                }
            }


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
        public QaTestRunDTO(Guid deviceTypeId, Dictionary<string, string> values)
        {
            Device = new DeviceDTO() {DeviceTypeId = deviceTypeId, Items = values};
        }
        
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
            Tests.Add(new TestDTO() {Values =  itemValues});
        }

    }

    public class TestDTO
    {
        public int TestNumber { get; set; }
        public Dictionary<string, string> Values { get; set; }
        public Dictionary<string, string> EndValues { get; set; } = new Dictionary<string, string>();
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
                { 4, Guid.Parse("f29d8c85-8183-4256-9b14-369ef440ec60")},
                { 99, Guid.Parse("8b247a7e-0ed2-41d4-a6b9-45834b7f14e1")},
                { 3, Guid.Parse("ced367b2-58dd-4ddd-bccf-43662f4ab36d")}
        };
    }
}