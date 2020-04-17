using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Devices.Core.Repository;
using Newtonsoft.Json;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.Models.EvcVerifications.Verifications;
using Prover.Application.Models.EvcVerifications.Verifications.CorrectionFactors;

namespace Prover.Application.FileLoader
{
    public class ItemLoader
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly JsonConverter<EvcVerificationTest> _jsonConverter;
        private readonly IVerificationTestService _testService;
        private JsonSerializerSettings _jsonOptions;

        public ItemLoader(IDeviceRepository deviceRepository, IVerificationTestService testService)
        {
            _deviceRepository = deviceRepository;
            _testService = testService;
            //_jsonConverter = new TestTemplateConverter(_deviceRepository, _testService);
            //_jsonOptions = new JsonSerializerSettings() ;
            //_jsonOptions.Converters.Add(_jsonConverter);
        }

        //private static JsonSerializerOptions _serializerOptions = new JsonSerialierOptions() { PropertyNameCaseInsensitive = true};

        public static async Task<string> GetFileInput() => await MessageInteractions.OpenFileDialog.Handle("Open file");

        public static async Task<ItemAndTestFile> LoadFromFile(IDeviceRepository devices, string filePath)
        {
            if (!File.Exists(filePath)) return null;
            var json = File.ReadAllText(filePath);
            var itemFile = JsonConvert.DeserializeObject<JsonItemAndTestFile>(json);
            var deviceType = devices.GetById(Guid.Parse(itemFile.DeviceId));

            if (deviceType == null)
                throw new NullReferenceException(nameof(deviceType));

            //var itemValues = deviceType.ToItemValues(itemFile.Items);
            var deviceInstance = deviceType.CreateInstance(itemFile.Items);
            var testNum = -1;

            var pressures = itemFile.PressureTests.Select(p => deviceType.ToItemValues(p))
                                    .ToDictionary(x =>
                                            {
                                                testNum++;
                                                return testNum;
                                            },
                                            values => values.ToList());
            testNum = -1;

            var temps = itemFile.TemperatureTests.Select(p => deviceType.ToItemValues(p))
                                .ToDictionary(x =>
                                        {
                                            testNum++;
                                            return testNum;
                                        },
                                        values => values.ToList());

            return new ItemAndTestFile
            {
                    Device = deviceInstance,
                    PressureTests = pressures,
                    TemperatureTests = temps
            };
        }

        public async Task<EvcVerificationTest> LoadTemplate(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            var serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            
            //serializer.Converters.Add(_jsonConverter);

            using (var streamer = File.OpenText(filePath))
            {
                var json = await streamer.ReadToEndAsync();
                var test = JsonConvert.DeserializeObject<VerificationTestTemplate>(json);

                var vm = _testService.NewVerification(test.Device);
                vm.SubmittedDateTime = DateTime.Now;
                return _testService.CreateModel(vm);
            }

            //var deviceType = devices.GetById(Guid.Parse(itemFile.DeviceId));
            //if (deviceType == null)
            //    throw new NullReferenceException(nameof(deviceType));

            ////var itemValues = deviceType.ToItemValues(itemFile.Items);
            //var deviceInstance = deviceType.CreateInstance(itemFile.Items);
        }

        #region Nested type: JsonItemAndTestFile

        private class JsonItemAndTestFile
        {
            public string DeviceId { get; set; }
            public Dictionary<string, string> Items { get; set; }

            public ICollection<Dictionary<string, string>> PressureTests { get; set; }
            public ICollection<Dictionary<string, string>> TemperatureTests { get; set; }
            public Dictionary<string, string> VolumeTest { get; set; }
        }

        #endregion
    }

    public class EvcVerificationFactory
    {
        private readonly EvcVerificationTest _instance;
        private DeviceInstance _device => _instance.Device;

        private EvcVerificationFactory(DeviceInstance device)
        {
            _instance = new EvcVerificationTest(device);
               
        }

        public static EvcVerificationFactory Create(DeviceInstance device)
        {
            return new EvcVerificationFactory(device);
          
            //ProverDefaults.TestDefinitions.ForEach(td =>
            //{
            //    var testPoint = new VerificationTestPoint(td.Level, new List<VerificationEntity>(), decimal.Zero);

            //    var values = template.TestPoints.FirstOrDefault(tp => tp.TestNumber == td.Level)?.ItemValues;

            //    if (template.Device.HasLiveTemperature())
            //    {
            //        testPoint.AddTests(new TemperatureCorrectionTest(template.Device.CreateItemGroup<TemperatureItems>(values), td.TemperatureGauge, decimal.Zero, decimal.Zero, 100m));
            //    }
            //    if (template.Device)
            //        case CompositionType.P:
            //            break;
            //        case CompositionType.PTZ:
            //            break;
            //        case CompositionType.Fixed:
            //            break;
            //        default:
            //            throw new ArgumentOutOfRangeException();
            //    }
            //});
        }

        public EvcVerificationFactory AddTestPoint(CorrectionTestDefinition testDefinition, ICollection<ItemValue> itemValues, int? testNumber = null)
        {
            testNumber = testNumber ?? _instance.Tests.Count;
            var tests = new List<VerificationEntity>();

            if (_device.HasLiveTemperature())
            {
                tests.Add(new TemperatureCorrectionTest(_device.CreateItemGroup<TemperatureItems>(itemValues), testDefinition.TemperatureGauge, decimal.Zero, decimal.Zero, 100m));
            }

            //if (_device.HasLivePressure())
            //{
            //    tests.Add(
            //            new PressureCorrectionTest(_device.CreateItemGroup<TemperatureItems>(itemValues), testDefinition.PressureGaugePercent, decimal.Zero, decimal.Zero));
            //}
            //if (template.Device)
            //        case CompositionType.P:
            //            break;
            //        case CompositionType.PTZ:
            //            break;
            //        case CompositionType.Fixed:
            //            break;
            //default:
            //            throw new ArgumentOutOfRangeException();

            return this;
        }
    
    }

    public class ItemAndTestFile
    {
        public DeviceInstance Device { get; set; }

        public Dictionary<int, List<ItemValue>> PressureTests { get; set; }
        public Dictionary<int, List<ItemValue>> TemperatureTests { get; set; }
    }

    public class VerificationTestTemplate
    {
        public DeviceInstance Device { get; set; }
        public ICollection<TestPoint> TestPoints { get; set; }

        #region Nested type: TestPoint
        public void UpdateTestPoints()
        {
            TestPoints.ForEach(tp =>
            {
                tp.ItemValues = Device.DeviceType.ToItemValues(tp.Values)
                                      .ToList();
            });
            //.SelectMany(tp => .A tp.ItemValues = v))
            //  .Select(v => )
        }
        public class TestPoint
        {
            public int TestNumber { get; set; }
            public Dictionary<string, string> Values { get; set; }

            [JsonIgnore]
            public ICollection<ItemValue> ItemValues { get; set; }
        }

        #endregion
    }
}


//  public class TestTemplateConverter : JsonConverter<EvcVerificationTest>
//  {
//      private readonly IDeviceRepository _deviceRepository;
//      private readonly IVerificationTestService _testService;

//      public TestTemplateConverter(IDeviceRepository deviceRepository, IVerificationTestService testService)
//      {
//          _deviceRepository = deviceRepository;
//          _testService = testService;
//      }


//      /// <inheritdoc />
//      //public override EvcVerificationTest ReadJson(JsonReader reader, Type objectType, EvcVerificationViewModel existingValue, bool hasExistingValue, JsonSerializer serializer)
//      //{
//      //    var random = new Random(DateTime.Now.Second);
//      //    var json = JObject.Load(reader);

//      //    var deviceType = _deviceRepository.GetById(Guid.Parse(json["deviceId"].Value<string>()));

//      //    var itemsJToken = json["items"];
//      //    var itemsDict = (Dictionary<string, string>)JsonConvert.DeserializeObject(json["items"].Value<string>(), typeof(Dictionary<string, string>));

//      //    var itemsDict = itemsJToken.Values<KeyValuePair<>>();
//      //            .Value<Dictionary<string, string>>();

//      //    var device = deviceType.CreateInstance(itemsDict);

//      //    var testVm = _testService.NewVerification(device);
//      //    //KeyValuePair<int, Dictionary<string, string>>
//      //    var testPoints = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<string, string>>>(json["testPoints"].Value<string>());

//      //    testPoints.ForEach(tp =>
//      //    {
//      //        if (device.HasLiveTemperature())
//      //            testVm.SetItems<TemperatureItems>(device, tp.Key, tp.Value);

//      //        if (device.HasLivePressure())
//      //            testVm.SetItems<PressureItems>(device, tp.Key, tp.Value);

//      //        if (device.HasLiveSuper())
//      //            testVm.SetItems<SuperFactorItems>(device, tp.Key, tp.Value);
//      //    });

//      //    return testVm;
//      //}

//      /// <inheritdoc />
//      public override void WriteJson(JsonWriter writer, EvcVerificationTest value, JsonSerializer serializer)
//      {
//          throw new NotImplementedException();
//      }

//      /// <inheritdoc />
//      public override EvcVerificationTest ReadJson(JsonReader reader, Type objectType, EvcVerificationTest existingValue, bool hasExistingValue, JsonSerializer serializer)
//      {
//          var random = new Random(DateTime.Now.Second);

//          // deserialize JSON directly from a file

//          var json = (string) reader.Value;

//          var device = JsonConvert.DeserializeObject<DeviceInstance>(json);

//          var test = new EvcVerificationTest();
//          JsonConvert.PopulateObject((string)reader.Value, test);
////var device = new DeviceInstanceJsonConverter(_deviceRepository)
//          //      .ReadJson(reader, typeof(DeviceInstance), null, false, serializer);

//         // var testPoints = json["testPoints"].Values<string>();
//          //var deviceType = _deviceRepository.GetById(Guid.Parse(json["deviceId"].Value<string>()))

//          //var itemsJToken = json["items"];
//          //var itemsDict = (Dictionary<string, string>)JsonConvert.DeserializeObject(json["items"].Value<string>(), typeof(Dictionary<string, string>));
//          return test;
//      }




//      /// <inheritdoc />
//      public override bool CanConvert(Type objectType) => throw new NotImplementedException();
//  }