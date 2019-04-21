using Core.Domain;
using System;
using System.Reflection;

namespace Module.EvcVerification.Models.CorrectionTestAggregate
{
    public class FrequencyTest : BaseEntity, IHavePercentError
    {
        #region Constructors

        public FrequencyTest() { }

        public FrequencyTest(CorrectionTest verificationTest)
        {
            VerificationTest = verificationTest;
            VerificationTestId = verificationTest.Id;
        }

        #endregion

        #region Properties

        public double? AdjustedVolumePercentError
        {
            get
            {
                if (AdjustedVolume() == 0) return null;

                var result = (EvcAdjustedVolume() - AdjustedVolume()) / AdjustedVolume() * 100;
                return result.HasValue ? double.Round(result.Value, 2) : default(double?);
            }
        }

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

        public double? PercentError { get; }

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

        public double? UnadjustedVolumePercentError
        {
            get
            {
                if (UnadjustedVolume() == 0) return null;

                var result = (EvcUnadjustedVolume() - UnadjustedVolume()) / UnadjustedVolume() * 100;
                return result.HasValue ? double.Round(result.Value, 2) : default(double?);
            }
        }

        [Required]
        public virtual CorrectionTest VerificationTest { get; set; }

        public Guid VerificationTestId { get; set; }

        #endregion

        #region Methods

        public double AdjustedVolume()
        {
            var mainAdjVol = MainRotorPulseCount / VerificationTest.Instrument.Items.GetItem(865).NumericValue;
            var senseAdjVol = SenseRotorPulseCount / VerificationTest.Instrument.Items.GetItem(866).NumericValue;
            return double.Round(mainAdjVol - senseAdjVol, 4);
        }

        public double? EvcAdjustedVolume()
        {
            var result = (PostTestItemValues?.AdjustedVolumeReading - PreTestItemValues?.AdjustedVolumeReading) * VerificationTest.Instrument.Items.GetItem(98).NumericValue;

            return result != null ? double.Round(result.Value, 4) : default(double?);
        }

        public double? EvcUnadjustedVolume()
        {
            var result = (PostTestItemValues?.UnadjustVolumeReading - PreTestItemValues?.UnadjustVolumeReading) * VerificationTest.Instrument.Items.GetItem(98).NumericValue;
            //
            return result != null ? double.Round(result.Value, 4) : default(double?);
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

        public double UnadjustedVolume()
        {
            if (MainRotorPulseCount == 0 || MechanicalOutputFactor == 0) return 0m;

            return double.Round((double)MainRotorPulseCount / MechanicalOutputFactor, 4);
        }

        #endregion

        #region Fields

        private string _itemsType;

        private string _postTestItemData;

        private string _preTestItemData;

        #endregion
    }
}