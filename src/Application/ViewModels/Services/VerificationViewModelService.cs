using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Services;
using AutoMapper;
using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Domain.EvcVerifications;
using Domain.EvcVerifications.Builders;
using Domain.EvcVerifications.Verifications.CorrectionFactors;
using Domain.EvcVerifications.Verifications.Volume.InputTypes;
using Shared.Interfaces;

namespace Application.ViewModels.Services
{
    public class VerificationViewModelService
    {
        private readonly IAsyncRepository<EvcVerificationTest> _evcVerificationRepository;

        private readonly Mapper _mapper;

        private readonly List<CorrectionTestDefinition> _testDefinitions = new List<CorrectionTestDefinition>
        {
            new CorrectionTestDefinition
            {
                Level = 0, TemperatureGauge = 32, PressureGaugePercent = 80, IsVolumeTest = true
            },
            new CorrectionTestDefinition
            {
                Level = 1, TemperatureGauge = 60, PressureGaugePercent = 50, IsVolumeTest = false
            },
            new CorrectionTestDefinition
            {
                Level = 2, TemperatureGauge = 90, PressureGaugePercent = 20, IsVolumeTest = false
            }
        };

        public VerificationViewModelService(IAsyncRepository<EvcVerificationTest> evcVerificationRepository)
        {
            _evcVerificationRepository = evcVerificationRepository;
            _mapper = Mappers.Setup();
        }

        #region Public Methods

        public EvcVerificationTest ConvertViewModelToModel(EvcVerificationViewModel viewModel)
        {
            var testPoints = viewModel.Tests.ToList().Select(t =>
            {
                var tp = _mapper.Map<VerificationTestPointViewModel, VerificationTestPoint>(t);

                tp.AddTest(_mapper.Map<PressureCorrectionTest>(t.Pressure));
                tp.AddTest(_mapper.Map<TemperatureCorrectionTest>(t.Temperature));
                tp.AddTest(_mapper.Map<SuperCorrectionTest>(t.SuperFactor));

                return tp;
            }).ToList();

            var evc = _mapper.Map<EvcVerificationTest>(viewModel);

            evc.Tests.Clear();
            evc.AddTests(testPoints);

            return evc;
        }

        public async Task<EvcVerificationViewModel> CreateNewTest(DeviceInstance device)
        {
            var builder = new EvcVerificationBuilder(device);
            //var vms = new List<VerificationTestPointViewModel>();

            //_testDefinitions.ForEach(td => vms.Add(
            //    BuildVerificationTestPointViewModel(device, td)
            //));

            return new EvcVerificationViewModel
            {
                Device = device,
                DeviceType = device.DeviceType,
                DriveType = VolumeInputTypes.Create(device),
                TestDateTime = DateTimeOffset.Now,
                Tests =
                    _testDefinitions.Select(td => BuildVerificationTestPointViewModel(device, td)).ToList()
            };
        }

        public async Task<EvcVerificationViewModel> CreateViewModelFromVerification(EvcVerificationTest test)
        {
            var testPoints = test.Tests.OfType<VerificationTestPoint>()
                .ToList().Select(tp =>
                {
                    var tpVm = _mapper.Map<VerificationTestPoint, VerificationTestPointViewModel>(tp);

                    var pressure = _mapper.Map<PressureFactorViewModel>(tp.GetTest<PressureCorrectionTest>());
                    tpVm.TestsCollection.Add(pressure);

                    var temp = _mapper.Map<TemperatureFactorViewModel>(tp.GetTest<TemperatureCorrectionTest>());
                    tpVm.TestsCollection.Add(temp);


                    tpVm.TestsCollection.Add(
                        _mapper.Map<SuperFactorViewModel>(tp.GetTest<SuperCorrectionTest>()));

                    return tpVm;
                }).ToList();

            var evc = _mapper.Map<EvcVerificationViewModel>(test);

            evc.Tests.Clear();
            evc.Tests.ToList().AddRange(testPoints);

            return evc;
        }

        #endregion

        #region Protected

        protected virtual VerificationTestPointViewModel BuildVerificationTestPointViewModel(DeviceInstance device,
            CorrectionTestDefinition definition)
        {
            var tpViewModel = new VerificationTestPointViewModel
            {
                TestNumber = definition.Level
            };

            var compType = device.ItemGroup<SiteInformationItems>().CompositionType;

            if (compType == CompositionType.P || compType == CompositionType.PTZ)
                tpViewModel.TestsCollection.Add(
                    MakePressureVieWModel(device.ItemGroup<PressureItems>(), definition.PressureGaugePercent, 0)
                );

            if (compType == CompositionType.T || compType == CompositionType.PTZ)
                tpViewModel.TestsCollection.Add(
                    MakeTemperatureViewModel(device.ItemGroup<TemperatureItems>(), definition.TemperatureGauge)
                );

            if (compType == CompositionType.PTZ)
                tpViewModel.TestsCollection.Add(
                    MakeSuperFactorViewModel(device.ItemGroup<SuperFactorItems>(), tpViewModel.Temperature,
                        tpViewModel.Pressure)
                );


            if (definition.IsVolumeTest)
                tpViewModel.TestsCollection.Add(
                    MakeVolumeViewModel(device, device.ItemGroup<VolumeItems>(), device.ItemGroup<VolumeItems>()));

            return tpViewModel;
        }

        #endregion

        #region Private

        private PressureFactorViewModel MakePressureVieWModel(PressureItems items, decimal gauge, decimal atmPressure)
        {
            if (items != null)
                return new PressureFactorViewModel(items, gauge, atmPressure);

            return null;
        }

        private SuperFactorViewModel MakeSuperFactorViewModel(SuperFactorItems items,
            TemperatureFactorViewModel temperature, PressureFactorViewModel pressure)
        {
            if (items != null && pressure != null && temperature != null)
                return new SuperFactorViewModel(items, temperature, pressure);

            return null;
        }

        private TemperatureFactorViewModel MakeTemperatureViewModel(TemperatureItems items, decimal gauge)
        {
            if (items != null)
                return new TemperatureFactorViewModel(items, gauge);

            return null;
        }

        private VolumeViewModel MakeVolumeViewModel(DeviceInstance device, VolumeItems startItems, VolumeItems endItems)
        {
            return new VolumeViewModel
            {
                AppliedInput = 0,
                StartItems = device.ItemGroup<VolumeItems>(),
                EndItems = device.ItemGroup<VolumeItems>()
            };
        }

        #endregion
    }
}