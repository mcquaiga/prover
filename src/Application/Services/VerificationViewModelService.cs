using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Application.Config;
using Application.ViewModels;
using Application.ViewModels.Corrections;
using Application.ViewModels.Services;
using AutoMapper;
using Devices.Core.Interfaces;
using Domain.EvcVerifications;
using Domain.EvcVerifications.Verifications;
using Domain.EvcVerifications.Verifications.CorrectionFactors;
using Shared.Interfaces;

namespace Application.Services
{
    public static class VerificationMapper
    {
        private static readonly Mapper _mapper = Mappers.Setup();

        public static EvcVerificationTest ToModel(this EvcVerificationViewModel viewModel)
        {
            return MapViewModelToModel(viewModel);
        }

        public static EvcVerificationViewModel ToViewModel(this EvcVerificationTest model)
        {
            return MapModelToViewModel(model);
        }

        public static EvcVerificationTest MapViewModelToModel(EvcVerificationViewModel viewModel)
        {
            var testPoints = viewModel.Tests.ToList().Select(vm =>
            {
                var tp = _mapper.Map<VerificationTestPointViewModel, VerificationTestPoint>(vm);

                tp.AddTests(_mapper.Map<PressureCorrectionTest>(vm.Pressure));
                tp.AddTests(_mapper.Map<TemperatureCorrectionTest>(vm.Temperature));
                tp.AddTests(_mapper.Map<SuperCorrectionTest>(vm.SuperFactor));

                if (vm.Volume != null)
                {
                    var volumeTests = vm.Volume?.AllTests().Select(v =>
                    {
                        var destinationType = _mapper.DefaultContext.ConfigurationProvider.GetAllTypeMaps()
                            .First(map => map.SourceType == v.GetType()).DestinationType;

                        if (destinationType == null)
                            throw new Exception($"Couldn't find a mapping for source type {v.GetType()}.");

                        var obj = Activator.CreateInstance(destinationType, true);
                        return (VerificationEntity) _mapper.Map(v, obj, v.GetType(), destinationType);
                    }).ToList();

                    tp.AddTests(volumeTests);
                }

                return tp;
            }).ToList();

            var evc = _mapper.Map<EvcVerificationTest>(viewModel);

            evc.Tests.Clear();
            evc.AddTests(testPoints);

            return evc;
        }

        public static EvcVerificationViewModel MapModelToViewModel(EvcVerificationTest test)
        {
            var evcViewModel = _mapper.Map<EvcVerificationViewModel>(test);

            var testPoints = test.Tests.OfType<VerificationTestPoint>()
                .ToList().Select(point =>
                {
                    var pointViewModel = _mapper.Map<VerificationTestPoint, VerificationTestPointViewModel>(point);

                    var pressure = _mapper.Map<PressureFactorViewModel>(point.GetTest<PressureCorrectionTest>());
                    pointViewModel.TestsCollection.Add(pressure);

                    var temp = _mapper.Map<TemperatureFactorViewModel>(point.GetTest<TemperatureCorrectionTest>());
                    pointViewModel.TestsCollection.Add(temp);

                    var superModel = point.GetTest<SuperCorrectionTest>();

                    var super = _mapper.Map<SuperFactorViewModel>(superModel);
                    super.Setup(temp, pressure);

                    pointViewModel.TestsCollection.Add(super);

                    if (point.HasVolume())
                    {
                        VolumeViewModelFactory.Create(test.Device, evcViewModel, pointViewModel);
                        var volumeTests = pointViewModel.Volume.AllTests().Select(vm =>
                        {
                            var sourceType = _mapper.DefaultContext.ConfigurationProvider.GetAllTypeMaps()
                                .First(map => map.DestinationType == vm.GetType()).SourceType;

                            var modelInstance = point.Tests.FirstOrDefault(t => t.GetType() == sourceType);

                            var result = (VerificationViewModel)_mapper.Map(modelInstance, vm, sourceType, vm.GetType());
                            vm = result;
                            return result;
                        }).ToList();
                    }

                    return pointViewModel;
                }).ToList();

            evcViewModel.Tests.Clear();
            testPoints.ForEach(evcViewModel.Tests.Add);
            return evcViewModel;
        }
    }

    public class VerificationViewModelService
    {
        private readonly IAsyncRepository<EvcVerificationTest> _evcVerificationRepository;

        private readonly VerificationViewModelTestCreator _verificationTestCreator;

        public VerificationViewModelService(IAsyncRepository<EvcVerificationTest> evcVerificationRepository)
        {
            _evcVerificationRepository = evcVerificationRepository;
            _verificationTestCreator = new VerificationViewModelTestCreator();

        }

        public EvcVerificationTest CreateVerificationTestFromViewModel(EvcVerificationViewModel viewModel) => VerificationMapper.MapViewModelToModel(viewModel);

        public async Task<EvcVerificationViewModel> GetVerificationTest(
            EvcVerificationTest verificationTest)
        {
            return await Task.Run(() => VerificationMapper.MapModelToViewModel(verificationTest));
        }

        public async Task<ICollection<EvcVerificationViewModel>> GetVerificationTests(
            IEnumerable<EvcVerificationTest> verificationTests)
        {
            var evcTests = new ConcurrentBag<EvcVerificationViewModel>();

            foreach (var model in verificationTests)
            {
                await Task.Run(() 
                    => evcTests.Add(VerificationMapper.MapModelToViewModel(model)));
            }

            return evcTests.ToList();
        }

        public EvcVerificationViewModel NewTest(DeviceInstance device)
        {
            var testModel = new EvcVerificationTest(device);

            var  testViewModel = VerificationMapper.MapModelToViewModel(testModel);

            return _verificationTestCreator.BuildEvcVerificationViewModel(testModel);
        }
    }
}