using Devices.Core.Interfaces;
using Devices.Core.Interfaces.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using static Module.EvcVerification.CorrectionTestDefinition;

namespace Module.EvcVerification.Models.DriveTypes
{
    public class Energy
    {
        #region Constructors

        public Energy(IDevice device)
        {
            _instrument = instrument;
            EnergyUnits = (Units)Enum.Parse(typeof(Units), _instrument.Items.GetItem(141).Description);
        }

        public Energy(Units unitValue, double startValue, double endValue, double totalValue, double evcCorrected)
        {
            StartValue = startValue;
            EndValue = endValue;
            TotalValue = totalValue;
            EnergyUnits = unitValue;
            _evcCorrected = evcCorrected;
        }

        #endregion

        #region Enums

        public enum Units
        {
            Therms,

            Dktherms,

            MegaJoules,

            GigaJoules,

            KiloCals,

            KiloWattHours
        }

        #endregion

        #region Properties

        public double? ActualEnergy
        {
            get
            {
                if (!_evcCorrected.HasValue && _instrument != null && !_instrument.VolumeTest.EvcCorrected.HasValue
                    && !_instrument.VolumeTest.EvcCorrected.HasValue && _instrument.VolumeTest.EvcCorrected == 0)
                {
                    return 0.0m;
                }

                if (_instrument != null)
                {
                    if (!_instrument.VolumeTest.EvcCorrected.HasValue)
                        return null;

                    _evcCorrected = _instrument.VolumeTest?.EvcCorrected.Value;
                    TotalValue = _instrument.Items.GetItem(142).NumericValue;
                }

                switch (EnergyUnits)
                {
                    case Units.Therms:
                        return Math.Round(TotalValue * _evcCorrected.Value / 100000);

                    case Units.Dktherms:
                        return Math.Round(TotalValue * _evcCorrected.Value / 1000000);

                    case Units.GigaJoules:
                        return Math.Round(TotalValue * 0.028317m * _evcCorrected.Value / 1000000);

                    case Units.MegaJoules:
                        return Math.Round(TotalValue * 0.028317m * _evcCorrected.Value / 1000);

                    case Units.KiloCals:
                        return Math.Round(TotalValue * 0.0283168m * _evcCorrected.Value);

                    default:
                        throw new Exception(string.Format("Energy units not supported: {0}", EnergyUnits));
                }
            }
        }

        public double? EndValue { get; private set; }

        public Units EnergyUnits { get; }

        public double? EvcEnergy
        {
            get
            {
                if (_instrument != null)
                {
                    StartValue = _instrument.VolumeTest.Items?.GetItem(140)?.NumericValue;
                    EndValue = _instrument.VolumeTest.AfterTestItems?.GetItem(140)?.NumericValue;
                }

                if (StartValue.HasValue && EndValue.HasValue)
                {
                    return EndValue.Value - StartValue.Value;
                }

                return null;
            }
        }

        public bool HasPassed => PercentError < 1 && PercentError > -1;

        public double? PercentError
        {
            get
            {
                if (ActualEnergy == 0) return null;
                var error = (EvcEnergy - ActualEnergy) / ActualEnergy * 100;
                if (error != null)
                    return double.Round(error.Value, 2);
                return null;
            }
        }

        public double? StartValue { get; private set; }

        public double TotalValue { get; private set; }

        #endregion

        #region Fields

        private readonly EvcVerification _instrument;

        private double? _evcCorrected;

        #endregion
    }

    public class MechanicalDrive : IDriveType
    {
        #region Constructors

        public MechanicalDrive(IDevice device)
        {
            Device = device;
            Energy = new Energy(device);
        }

        public MechanicalDrive(IDevice device, List<MechanicalUncorrectedTestLimit> mechanicalUncorrectedTestLimits)
            : this(device)
        {
            _mechanicalUncorrectedTestLimits = mechanicalUncorrectedTestLimits;
        }

        #endregion

        #region Properties

        public IDevice Device { get; }

        public string Discriminator => Drives.Mechanical;

        public Energy Energy { get; set; }

        public bool HasPassed => Energy.HasPassed;

        #endregion

        #region Methods

        public int MaxUncorrectedPulses()
        {
            var uncorUnitValue = (int)Device.GetItem(98).NumericValue;

            return _mechanicalUncorrectedTestLimits?
                       .FirstOrDefault(x => x.CuFtValue == uncorUnitValue)?.UncorrectedPulses ?? 10;
        }

        public double? UnCorrectedInputVolume(double appliedInput)
        {
            return appliedInput * Device.DriveRate();
        }

        #endregion

        #region Fields

        private readonly List<MechanicalUncorrectedTestLimit> _mechanicalUncorrectedTestLimits;

        #endregion
    }
}