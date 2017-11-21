using Newtonsoft.Json;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.Items;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Prover.Core.Models.Instruments
{
    public sealed class FrequencyTest : BaseVerificationTest
    {
        private string _afterTestData;

        public FrequencyTest() { }

        public FrequencyTest(VerificationTest verificationTest)
        {
            Items = verificationTest.Instrument.Items.Where(i => i.Metadata.IsFrequencyTest == true).ToList();
            VerificationTest = verificationTest;
            VerificationTestId = verificationTest.Id;
        }

        [NotMapped]
        public IEnumerable<ItemValue> AfterTestItems { get; set; }
        public string AfterTestData
        {
            get { return AfterTestItems.Serialize(); }
            set { _afterTestData = value; }
        }

        public override decimal? PercentError
        {
            get
            {
                if (AdjustedVolume() == 0) return null;

                return (EvcAdjustedVolume() - AdjustedVolume()) / AdjustedVolume() * 100;
            }
        }

        public override decimal? ActualFactor => 0m;
        public override decimal? EvcFactor { get; }
        public long MainRotorPulseCount { get; set; }
        public long SenseRotorPulseCount { get; set; }
        public long MechanicalOutputFactor { get; set; }

        public decimal AdjustedVolume()
        {
            var mainAdjVol = MainRotorPulseCount / VerificationTest.Instrument.Items.GetItem(865).NumericValue;
            var senseAdjVol = SenseRotorPulseCount / VerificationTest.Instrument.Items.GetItem(866).NumericValue;
            return mainAdjVol - senseAdjVol;
        }

        public long RoundedAdjustedVolume()
        {
            var indexRate = (long) VerificationTest.Instrument.Items.GetItem(98).NumericValue;
            var result = (long) (AdjustedVolume() / indexRate);
            return result * indexRate;
        }

        public decimal UnadjustedVolume()
        {
            if (MainRotorPulseCount == 0 || MechanicalOutputFactor == 0) return 0m;

            return (decimal) MainRotorPulseCount / MechanicalOutputFactor;
        }

        public decimal EvcAdjustedVolume()
        {
            return AfterTestItems.GetItem(850).NumericValue - Items.GetItem(850).NumericValue;
        }

        public decimal EvcUnadjustedVolume()
        {
            return AfterTestItems.GetItem(852).NumericValue - Items.GetItem(852).NumericValue;
        }

        public override void OnInitializing()
        {
            base.OnInitializing();

            if (!string.IsNullOrEmpty(_afterTestData))
            {
                var afterItemValues = JsonConvert.DeserializeObject<Dictionary<int, string>>(_afterTestData);
                AfterTestItems = ItemHelpers.LoadItems(VerificationTest.Instrument.InstrumentType, afterItemValues);
            }
        }
    }
}
