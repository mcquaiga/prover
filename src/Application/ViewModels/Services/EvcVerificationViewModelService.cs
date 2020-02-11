using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Domain.EvcVerifications;
using Domain.EvcVerifications.Builders;
using Domain.EvcVerifications.CorrectionTests;
using Shared.Interfaces;

namespace Application.ViewModels.Services
{
    public class EvcVerificationViewModelService
    {
        private readonly IAsyncRepositoryGuid<EvcVerificationTest> _evcVerificationRepository;

        private readonly List<CorrectionTestDefinition> _testDefinitions = new List<CorrectionTestDefinition>
        {
            new CorrectionTestDefinition()
            {
                Level = 0, TemperatureGauge = 32, PressureGaugePercent = 80, IsVolumeTest = true
            },
            new CorrectionTestDefinition()
            {
                Level = 1, TemperatureGauge = 60, PressureGaugePercent = 50, IsVolumeTest = false
            },
            new CorrectionTestDefinition()
            {
                Level = 2, TemperatureGauge = 90, PressureGaugePercent = 20, IsVolumeTest = false
            }
        };
            

        public EvcVerificationViewModelService(IAsyncRepositoryGuid<EvcVerificationTest> evcVerificationRepository)
        {
            _evcVerificationRepository = evcVerificationRepository;
        }

        public EvcVerificationViewModelService()
        {

        }

        public async Task<EvcVerificationViewModel> CreateNewVerificationTest(DeviceInstance device)
        {
            var builder = new EvcVerificationBuilder(device);
            
            var testPointViewModels = MakeTests(builder, device);

            var evc = builder.GetEvcVerification();
            return new EvcVerificationViewModel()
            {
                Id = evc.Id,
                Device = device,
                DeviceType = device.DeviceType,
                DriveType = evc.DriveType,
                TestDateTime = evc.TestDateTime,
                Tests = testPointViewModels.ToList()
                
            };
        }

        private IEnumerable<VerificationTestPointViewModel> MakeTests(EvcVerificationBuilder builder, DeviceInstance device)
        {
            var vms = new List<VerificationTestPointViewModel>();

            _testDefinitions.ForEach(td =>
            {
                var testPointBuilder = builder.TestPointFactory()
                    .CreateNew(td.Level, device.Values, device.Values)
                        .BuildPressureTest(device.ItemGroup<IPressureItems>(), td.PressureGaugePercent)
                        .BuildTemperatureTest(device.ItemGroup<ITemperatureItems>(), td.TemperatureGauge)
                        .BuildSuperFactorTest(device.ItemGroup<ISuperFactorItems>());

                if (td.IsVolumeTest)
                {
                    testPointBuilder.BuildVolumeTest(device.ItemGroup<IVolumeItems>(), device.ItemGroup<IVolumeItems>(), 0);
                }

                var tp = testPointBuilder.Commit();

                vms.Add(
                    BuildVerificationTestPointViewModel(device, tp)
                );
            });

            return vms;
        }

        protected virtual VerificationTestPointViewModel BuildVerificationTestPointViewModel(DeviceInstance device,
            VerificationTestPoint testPoint)
        {
            var tpViewModel = new VerificationTestPointViewModel()
            {
                Id = testPoint.Id,
                Level = testPoint.TestNumber,
                BeforeValues = testPoint.BeforeValues,
                AfterValues = testPoint.AfterValues,

                Pressure = MakePressureVieWModel(
                    testPoint.GetCorrectionTest<CorrectionTestWithGauge>(CorrectionFactorTestType.Pressure),
                    device.ItemGroup<IPressureItems>(testPoint.BeforeValues)),

                Temperature = MakeTemperatureViewModel(
                    testPoint.GetCorrectionTest<CorrectionTestWithGauge>(CorrectionFactorTestType.Temperature),
                    device.ItemGroup<ITemperatureItems>(testPoint.BeforeValues)),
            };

            tpViewModel.SuperFactor = MakeSuperFactorViewModel(
                testPoint.GetCorrectionTest<CorrectionTest>(CorrectionFactorTestType.Super),
                device.ItemGroup<ISuperFactorItems>(testPoint.BeforeValues), tpViewModel.Temperature, tpViewModel.Pressure);


            if (testPoint.HasVolume())
            {
                tpViewModel.Volume = new VolumeViewModel()
                {
                    AppliedInput = 0,
                    StartItems = device.ItemGroup<IVolumeItems>(testPoint.BeforeValues),
                    EndItems = device.ItemGroup<IVolumeItems>(testPoint.AfterValues)
                };
            }

            return tpViewModel;
        }

        private SuperFactorViewModel MakeSuperFactorViewModel(CorrectionTest correctionTest, ISuperFactorItems items, TemperatureFactorViewModel temperature, PressureFactorViewModel pressure)
        {
            if (correctionTest != null && pressure != null && temperature != null)
                return new SuperFactorViewModel(items, temperature, pressure);

            return null;
        }

        private TemperatureFactorViewModel MakeTemperatureViewModel(CorrectionTest correctionTest, ITemperatureItems items)
        {
            if (correctionTest != null && correctionTest is CorrectionTestWithGauge ct)
                return new TemperatureFactorViewModel(items, ct.Gauge);

            return null;
        }

        private PressureFactorViewModel MakePressureVieWModel(CorrectionTest correctionTest, IPressureItems items)
        {
            if (correctionTest != null && correctionTest is CorrectionTestWithGauge ct)
                return new PressureFactorViewModel(items, ct.Gauge, items.AtmosphericPressure);

            return null;
        }
    }
}
