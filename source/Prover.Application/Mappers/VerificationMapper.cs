using AutoMapper;
using Prover.Application.Extensions;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.Models.EvcVerifications.Verifications;
using Prover.Application.Models.EvcVerifications.Verifications.CorrectionFactors;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Corrections;
using Prover.Application.ViewModels.Factories.Volume;
using System;
using System.Linq;

namespace Prover.Application.Mappers
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
            var testPoints = viewModel.VerificationTests.OfType<VerificationTestPointViewModel>().ToList().Select(vm =>
            {
                var tp = _mapper.Map<VerificationTestPointViewModel, VerificationTestPoint>(vm);

                tp.AddTest(_mapper.Map<PressureCorrectionTest>(vm.GetPressureTest()));
                tp.AddTest(_mapper.Map<TemperatureCorrectionTest>(vm.GetTemperatureTest()));
                tp.AddTest(_mapper.Map<SuperCorrectionTest>(vm.GetSuperFactorTest()));

                if (vm.GetVolumeTest() != null)
                {
                    var volumeTests = vm.GetVolumeTest()?.AllTests().Select(v =>
                    {
                        var destinationType = _mapper.DefaultContext.ConfigurationProvider.GetAllTypeMaps()
                            .First(map => map.SourceType == v.GetType()).DestinationType;

                        if (destinationType == null)
                            throw new Exception($"Couldn't find a mapping for source type {v.GetType()}.");

                        var obj = Activator.CreateInstance(destinationType, true);
                        var instance = (VerificationEntity)_mapper.Map(v, obj, v.GetType(), destinationType);
                        //if (instance is IPulseOutputVerification)
                        //{
                        //    if (v is VolumeTestRunViewModelBase vmBase)
                        //        (instance as IPulseOutputVerification).PulseOutputTest = _mapper.Map<PulseOutputVerification>(vmBase.PulseOutputTest);
                        //}

                        return instance;
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
                    if (pressure != null)
                        pointViewModel.VerificationTests.Add(pressure);

                    var temp = _mapper.Map<TemperatureFactorViewModel>(point.GetTest<TemperatureCorrectionTest>());
                    if (temp != null)
                        pointViewModel.VerificationTests.Add(temp);

                    var superModel = point.GetTest<SuperCorrectionTest>();
                    if (superModel != null)
                    {
                        var super = _mapper.Map<SuperFactorViewModel>(superModel);
                        super.Setup(temp, pressure);

                        pointViewModel.VerificationTests.Add(super);
                    }

                    if (point.HasVolumeTest())
                    {
                        VolumeViewModelFactory.Create(test.Device, evcViewModel, pointViewModel);
                        var volumeTests = pointViewModel.GetVolumeTest().AllTests().Select(vm =>
                        {
                            var sourceType = _mapper.DefaultContext.ConfigurationProvider.GetAllTypeMaps().FirstOrDefault(map => map.DestinationType == vm.GetType())?.SourceType;

                            var modelInstance = point.Tests.FirstOrDefault(t => t.GetType() == sourceType);

                            var result = (VerificationViewModel)_mapper.Map(modelInstance, vm, sourceType, vm.GetType());
                            return result;
                        }).ToList();
                    }
                    pointViewModel.Initialize();

                    return (VerificationViewModel)pointViewModel;
                }).ToList();

            evcViewModel.Initialize(testPoints.ToList());
            return evcViewModel;
        }
    }
}