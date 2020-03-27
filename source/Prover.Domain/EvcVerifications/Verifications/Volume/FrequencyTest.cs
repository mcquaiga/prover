using System;
using Devices.Core.Items.ItemGroups;
using Prover.Domain.Interfaces;
using Prover.Shared.Domain;

namespace Prover.Domain.EvcVerifications.Verifications.Volume
{
    public class FrequencyTest : BaseEntity, IAssertPassFail
    {
        private string _itemsType;

        public FrequencyTest()
        {
        }

        public FrequencyTest(VerificationTestPoint verificationTest)
        {
            VerificationTest = verificationTest;
        }

        #region Public Properties

        public decimal? AdjustedVolumePercentError
        {
            get
            {
                if (AdjustedVolume() == 0) return null;

                var result = (EvcAdjustedVolume() - AdjustedVolume()) / AdjustedVolume() * 100;
                return result.HasValue ? Math.Round(result.Value, 2) : default(decimal?);
            }
        }

        public IFrequencyTestItems EndValues { get; set; }

        public string ItemsType
        {
            get => StartValues?.GetType().AssemblyQualifiedName;
            set => _itemsType = value;
        }

        public long MainRotorPulseCount { get; set; }
        public long MechanicalOutputFactor { get; set; }

        public bool HasPassed() =>
            AdjustedVolumePercentError.HasValue && AdjustedVolumePercentError < 1 && AdjustedVolumePercentError > -1 &&
            UnadjustedVolumePercentError.HasValue && UnadjustedVolumePercentError < 1 &&
            UnadjustedVolumePercentError > -1;

        public decimal PassTolerance => throw new NotImplementedException();
        public decimal? PercentError { get; }
        public long SenseRotorPulseCount { get; set; }
        public IFrequencyTestItems StartValues { get; set; }

        public decimal? UnadjustedVolumePercentError
        {
            get
            {
                if (UnadjustedVolume() == 0) return null;

                var result = (EvcUnadjustedVolume() - UnadjustedVolume()) / UnadjustedVolume() * 100;
                return result.HasValue ? Math.Round(result.Value, 2) : default(decimal?);
            }
        }

        public virtual VerificationTestPoint VerificationTest { get; set; }
        public Guid VerificationTestId { get; set; }

        #endregion

        #region Public Methods

        public decimal AdjustedVolume()
        {
            ////var mainAdjVol = MainRotorPulseCount / VerificationTest.Instrument.Items.GetItem(865).NumericValue;
            ////var senseAdjVol = SenseRotorPulseCount / VerificationTest.Instrument.Items.GetItem(866).NumericValue;
            ////return Math.Round(mainAdjVol - senseAdjVol, 4);
            throw new NotImplementedException();
        }

        public decimal? EvcAdjustedVolume()
        {
            throw new NotImplementedException();
            //var result = (PostTestItemValues?.AdjustedVolumeReading - PreTestItemValues?.AdjustedVolumeReading) * VerificationTest.Instrument.Items.GetItem(98).NumericValue;

            //return result != null ? Math.Round(result.Value, 4) : default(decimal?);
        }

        public decimal? EvcUnadjustedVolume()
        {
            throw new NotImplementedException();
            //var result = (PostTestItemValues?.UnadjustVolumeReading - PreTestItemValues?.UnadjustVolumeReading) * VerificationTest.Instrument.Items.GetItem(98).NumericValue;
            ////
            //return result != null ? decimal.Round(result.Value, 4) : default(decimal?);
        }

        public long RoundedAdjustedVolume()
        {
            throw new NotImplementedException();
            //var indexRate = (long)VerificationTest.Instrument.Items.GetItem(98).NumericValue;
            //var result = (long)(AdjustedVolume() / indexRate);
            //return result * indexRate;
        }

        //    var type = Type.GetType(_itemsType, name => Assembly.Load(name), (assembly, s, arg3) => assembly.GetType(s));
        //    if (type != null && !string.IsNullOrEmpty(_preTestItemData) && !string.IsNullOrEmpty(_postTestItemData))
        //    {
        //        PreTestItemValues = (IFrequencyTestItems)JsonConvert.DeserializeObject(_preTestItemData, type);
        //        PostTestItemValues = (IFrequencyTestItems)JsonConvert.DeserializeObject(_postTestItemData, type);
        //    }
        //}
        public decimal UnadjustedVolume()
        {
            if (MainRotorPulseCount == 0 || MechanicalOutputFactor == 0) return 0m;

            return Math.Round((decimal) MainRotorPulseCount / MechanicalOutputFactor, 4);
        }

        bool IAssertPassFail.HasPassed()
        {
            throw new NotImplementedException();
        }

        #endregion

        //public override void OnInitializing()
        //{
        //    base.OnInitializing();

        //    if (string.IsNullOrEmpty(_itemsType)) return;
    }
}