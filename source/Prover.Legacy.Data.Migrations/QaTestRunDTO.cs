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
using Prover.Application.Models.EvcVerifications;
using Prover.Application.Models.EvcVerifications.Builders;
using Prover.Application.ViewModels;
using Prover.Calculations;
using Prover.Shared;

namespace Prover.Legacy.Data.Migrations
{
    public class DataTransfer
    {
        private static SessionContext _session;
        private static List<DeviceType> _devices;

        static DataTransfer()
        {
            _session = new SessionContext();

            _devices = DeviceRepository.Instance.GetAll()
                                       .ToList();
        }

        public static DeviceType GetDevice(int deviceId)
        {
            return _devices.FirstOrDefault(d => d.Id == Mappers.DeviceTypeMappings[deviceId]);
        }

        public static Guid GetDeviceTypeId(int deviceId)
        {
            if (_devices == null)
            {
                var repo = DeviceRepository.Instance;

                _devices = repo.GetAll()
                               .ToList();
            }

            return Mappers.DeviceTypeMappings[deviceId];
        }

        public static async Task ImportTests(IVerificationTestService testService, string folderPath)
        {
            if (!Directory.Exists(folderPath)) throw new DirectoryNotFoundException($"{folderPath}");
            var testModels = new List<EvcVerificationTest>();

            foreach (var file in Directory.EnumerateFiles(folderPath))
                try
                {
                    Debug.WriteLine($"Loading {file}");

                    using (var reader = new StreamReader(file))
                    {
                        var json = await reader.ReadToEndAsync();

                        if (!string.IsNullOrEmpty(json))
                        {
                            var qaTest = JsonConvert.DeserializeObject<QaTestRunDTO>(json);
                            //Debug.WriteLine($"Device Id: {qaTest.Device.DeviceTypeId}");

                            if (qaTest.Device != null)
                            {
                                var deviceType = DeviceRepository.Instance.GetById(qaTest.Device.DeviceTypeId);
                                if (deviceType.Name == "Mini-Max")
                                {
                                    var device = deviceType.CreateInstance(qaTest.Device.Items);
                                    if (device.Items.Volume.VolumeInputType == VolumeInputType.Rotary)
                                    {
                                        var builder = device.BuildVerification();
                                        
                                        qaTest.Tests
                                              .OrderBy(x => x.TestNumber)
                                              .ForEach(vt =>
                                        {
                                            var testDef = VerificationTestOptions.Defaults.CorrectionTestDefinitions.First(x => x.Level == vt.TestNumber);

                                            builder.AddTestPoint(tp =>
                                            {
                                                var gaugePressure = device.Items.Pressure != null ? PressureCalculator.GetGaugePressure(device.Items.Pressure.Range, testDef.PressureGaugePercent) : 0m;
                                                var result = tp.Generate(gaugePressure, testDef.TemperatureGauge, vt.Values);

                                                if (vt.EndValues != null && testDef.IsVolumeTest == true)
                                                    result.WithVolume(vt.Values, vt.EndValues, vt.AppliedInput.ToInt32(), vt.CorPulses, vt.UnCorPulses);

                                                return result;
                                            });
                                        });
                                        var random = new Random(DateTime.Now.Millisecond);
                                        var model = builder.Build();

                                        model.TestDateTime = DateTime.Now.Subtract(TimeSpan.FromDays(random.Next(0, 30)));
                                        model.SubmittedDateTime = model.TestDateTime.AddSeconds(random.Next(180, 720));
                                        model.Verified = qaTest.IsPassed;
                                        await testService.AddOrUpdate(model);

                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    Console.WriteLine(ex);
                }
        }
    }

    internal class SessionContext
    {
        internal SessionContext() => TestItems = new List<QaTestRunDTO>();

        internal List<QaTestRunDTO> TestItems { get; }
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
        public QaTestRunDTO(Guid deviceTypeId, Dictionary<string, string> values) => Device = new DeviceDTO {DeviceTypeId = deviceTypeId, Items = values};

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
            Tests.Add(new TestDTO {Values = itemValues});
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