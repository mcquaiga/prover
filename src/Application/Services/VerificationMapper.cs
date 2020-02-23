using System;
using System.Linq;
using Application.Config;
using Application.ViewModels;
using Application.ViewModels.Corrections;
using Application.ViewModels.Services;
using AutoMapper;
using Domain.EvcVerifications;
using Domain.EvcVerifications.Verifications;
using Domain.EvcVerifications.Verifications.CorrectionFactors;

namespace Application.Services
{
    internal static class VerificationMapper
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
}