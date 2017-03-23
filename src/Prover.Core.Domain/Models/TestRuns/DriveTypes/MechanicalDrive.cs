using System;
using Prover.CommProtocol.Common.Instruments;

namespace Prover.Domain.Models.TestRuns.DriveTypes
{
    //public class MechanicalDrive : IDriveType
    //{
    //    public MechanicalDrive(IInstrument instrument)
    //    {
    //        Instrument = instrument;
    //        Energy = new Energy(instrument);
    //    }

    //    public IInstrument Instrument { get; set; }

    //    public Energy Energy { get; set; }

    //    public string Discriminator => "Mechanical";

    //    public bool HasPassed => Energy.HasPassed;

    //    public int MaxUncorrectedPulses()
    //    {
    //        var uncorPulseTable = SettingsManager.SettingsInstance.MechanicalUncorrectedTestLimits;
    //        var uncorUnitValue = (int) Instrument.VolumeItems.DriveRate;

    //        return uncorPulseTable.FirstOrDefault(x => x.CuFtValue == uncorUnitValue)?.UncorrectedPulses ?? 10;
    //    }

    //    decimal IDriveType.UnCorrectedInputVolume(decimal appliedInput)
    //    {
    //        return appliedInput * Instrument.DriveRate();
    //    }
    //}

    //public class Energy
    //{
    //    private readonly VolumeTestPoint _volumeTest;
    //    private const string Therms = "Therms";
    //    private const string Dktherms = "DecaTherms";
    //    private const string MegaJoules = " MegaJoules";
    //    private const string GigaJoules = "GigaJoules";
    //    private const string KiloCals = "KiloCals";
    //    private const string KiloWattHours = "KiloWattHours";

    //    private readonly IInstrument _instrument;

    //    public Energy(VolumeTestPoint volumeTest)
    //    {
    //        _volumeTest = volumeTest;
    //    }

    //    public bool HasPassed => PercentError < 1 && PercentError > -1;

    //    public decimal? PercentError
    //    {
    //        get
    //        {
    //            var error = (EvcEnergy - ActualEnergy) / ActualEnergy * 100;
    //            if (error != null)
    //                return decimal.Round(error.Value, 2);
    //            return null;
    //        }
    //    }

    //    public decimal? EvcEnergy
    //    {
    //        get
    //        {
    //            var startEnergy = _volumeTest.;
    //            var endEnergy = _instrument.VolumeTest.AfterTestItems?.GetItem(140)?.NumericValue;
    //            if (endEnergy != null && startEnergy != null)
    //                return endEnergy.Value - startEnergy.Value;

    //            return null;
    //        }
    //    }

    //    public string EnergyUnits => _instrument.Items.GetItem(141).Description;

    //    public decimal? ActualEnergy
    //    {
    //        get
    //        {
    //            var energyValue = _instrument.Items.GetItem(142).NumericValue;
    //            switch (EnergyUnits)
    //            {
    //                case Therms:
    //                    if (_instrument.VolumeTest.EvcCorrected.HasValue)
    //                        return Math.Round(energyValue * _instrument.VolumeTest.EvcCorrected.Value) / 100000;
    //                    break;
    //                case Dktherms:

    //                case GigaJoules:
    //                    break;
    //            }

    //            return null;
    //        }
    //    }
    //}
}