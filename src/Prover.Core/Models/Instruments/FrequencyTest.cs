using Newtonsoft.Json;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Shared.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Prover.Core.Models.Instruments
{
    public class FrequencyTest : EntityWithId, IHaveVerificationTest, IHavePercentError
    {
        public decimal? AdjustedCorrectedPercentError
        {
            get
            {
                if (AdjustedCorrectedVolume == 0) return null;

                var result = (EvcAdjustedCorrectedVolume - AdjustedCorrectedVolume) / AdjustedCorrectedVolume * 100;
                return decimal.Round(result, 2);
            }
        }

        public decimal AdjustedCorrectedVolume
        {
            get
            {
                return AdjustedVolume() * TotalCorrection().Value;
            }
        }

        public decimal? AdjustedVolumePercentError
        {
            get
            {
                if (AdjustedVolume() == 0) return null;

                var result = (TibAdjustedVolume() - AdjustedVolume()) / AdjustedVolume() * 100;
                return result.HasValue ? decimal.Round(result.Value, 2) : default(decimal?);
            }
        }

        public decimal EvcAdjustedCorrectedVolume
        {
            get
            {
                if (!TibAdjustedVolume().HasValue) return 0;

                return TibAdjustedVolume().Value * TotalCorrection().Value;
            }
        }

        public decimal? EvcAdjustedEndReading => PostTestItemValues?.MainAdjustedVolumeReading;

        public decimal? EvcAdjustedStartReading => PreTestItemValues?.MainAdjustedVolumeReading;

        [NotMapped]
        public bool HasPassed =>
            (AdjustedVolumePercentError.HasValue && AdjustedVolumePercentError < 1 && AdjustedVolumePercentError > -1)
        && (UnadjustedVolumePercentError.HasValue && UnadjustedVolumePercentError < 1 && UnadjustedVolumePercentError > -1);

        public string ItemsType
        {
            get => PreTestItemValues?.GetType().AssemblyQualifiedName;
            set => _itemsType = value;
        }

        public long MainRotorPulseCount { get; set; }

        public long MechanicalOutputFactor { get; set; }

        public decimal? PercentError { get; }

        public string PostTestItemData
        {
            get => JsonConvert.SerializeObject(PostTestItemValues);
            set => _postTestItemData = value;
        }

        [NotMapped]
        public IFrequencyTestItems PostTestItemValues { get; set; }

        public string PreTestItemData
        {
            get => JsonConvert.SerializeObject(PreTestItemValues);
            set => _preTestItemData = value;
        }

        [NotMapped]
        public IFrequencyTestItems PreTestItemValues { get; set; }

        public long SenseRotorPulseCount { get; set; }

        public decimal? TibAdjustedEndReading => PostTestItemValues?.TibAdjustedVolumeReading;

        public decimal? TibAdjustedStartReading => PreTestItemValues?.TibAdjustedVolumeReading;

        public decimal? UnadjustedEndReading => PostTestItemValues?.MainUnadjustVolumeReading;

        public decimal? UnadjustedStartReading => PreTestItemValues?.MainUnadjustVolumeReading;

        public decimal? UnadjustedVolumePercentError
        {
            get
            {
                if (UnadjustedVolume() == 0) return null;

                var result = (EvcUnadjustedVolume() - UnadjustedVolume()) / UnadjustedVolume() * 100;
                return result.HasValue ? decimal.Round(result.Value, 2) : default(decimal?);
            }
        }

        [Required]
        public virtual VerificationTest VerificationTest { get; set; }

        public Guid VerificationTestId { get; set; }

        public FrequencyTest()
        {
            //MainRotorPulseCount = 2600;
            //SenseRotorPulseCount = 260;
            MechanicalOutputFactor = 70;
        }

        public FrequencyTest(VerificationTest verificationTest)
        {
            VerificationTest = verificationTest;
            VerificationTestId = verificationTest.Id;
        }

        public decimal AdjustedVolume()
        {
            var mainAdjVol = MainRotorPulseCount / VerificationTest.Instrument.Items.GetItem(865).NumericValue;
            var senseAdjVol = SenseRotorPulseCount / VerificationTest.Instrument.Items.GetItem(866).NumericValue;
            return decimal.Round(mainAdjVol - senseAdjVol, 4);
        }

        public decimal? EvcUnadjustedVolume()
        {
            var result = (UnadjustedEndReading - UnadjustedStartReading) * VerificationTest.Instrument.Items.GetItem(98).NumericValue;
            return result != null ? decimal.Round(result.Value, 4) : default(decimal?);
        }

        public override void OnInitializing()
        {
            base.OnInitializing();

            if (string.IsNullOrEmpty(_itemsType)) return;

            var type = Type.GetType(_itemsType, name => Assembly.Load(name), (assembly, s, arg3) => assembly.GetType(s));
            if (type != null && !string.IsNullOrEmpty(_preTestItemData) && !string.IsNullOrEmpty(_postTestItemData))
            {
                PreTestItemValues = (IFrequencyTestItems)JsonConvert.DeserializeObject(_preTestItemData, type);
                PostTestItemValues = (IFrequencyTestItems)JsonConvert.DeserializeObject(_postTestItemData, type);
            }
        }

        public long RoundedAdjustedVolume()
        {
            var indexRate = (long)VerificationTest.Instrument.Items.GetItem(98).NumericValue;
            var result = (long)(AdjustedVolume() / indexRate);
            return result * indexRate;
        }

        public decimal? TibAdjustedVolume()
        {
            var result = (TibAdjustedEndReading - TibAdjustedStartReading) * VerificationTest.Instrument.Items.GetItem(98).NumericValue;

            return result != null ? decimal.Round(result.Value, 4) : default(decimal?);
        }

        public decimal? TotalCorrection()
        {
            return VerificationTest.SuperFactorTest.SuperFactorSquared * VerificationTest.PressureTest.ActualFactor
                * VerificationTest.TemperatureTest.ActualFactor;
        }

        public decimal UnadjustedVolume()
        {
            if (MainRotorPulseCount == 0 || MechanicalOutputFactor == 0) return 0m;

            return decimal.Round((decimal)MainRotorPulseCount / MechanicalOutputFactor, 4);
        }

        private string _itemsType;
        private string _postTestItemData;
        private string _preTestItemData;
    }
}