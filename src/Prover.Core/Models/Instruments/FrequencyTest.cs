using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.CommClients;
using Prover.CommProtocol.MiHoneywell.Items;
using Prover.Core.Extensions;

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
        public IEnumerable<ItemValue> AfterTestItems { get; set; } = new List<ItemValue>();
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

                var result = (EvcAdjustedVolume() - AdjustedVolume()) / AdjustedVolume() * 100;
                return result.HasValue ? decimal.Round(result.Value, 2) : default(decimal?);
            }
        }

        public override decimal? ActualFactor => 0m;
        public override decimal? EvcFactor => 0m;
        public long MainRotorPulseCount { get; set; }
        public long SenseRotorPulseCount { get; set; }
        public long MechanicalOutputFactor { get; set; }

        public decimal AdjustedVolume()
        {
            var mainAdjVol = MainRotorPulseCount / VerificationTest.Instrument.Items.GetItem(865).NumericValue;
            var senseAdjVol = SenseRotorPulseCount / VerificationTest.Instrument.Items.GetItem(866).NumericValue;
            return decimal.Round(mainAdjVol - senseAdjVol, 4);
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

            return decimal.Round((decimal) MainRotorPulseCount / MechanicalOutputFactor, 4);
        }

        public decimal? EvcAdjustedVolume()
        {
            if (!AfterTestItems.Any()) return default(decimal?);
            var startAdjusted = Items.GetHighResolutionValue(850, 851);
            var endAdjusted = AfterTestItems.GetHighResolutionValue(850, 851);
            return endAdjusted - startAdjusted;
        }

        public decimal? EvcUnadjustedVolume()
        {
            return AfterTestItems?.GetItem(852)?.NumericValue - Items?.GetItem(852)?.NumericValue;
        }

        [NotMapped]
        public override InstrumentType InstrumentType => VerificationTest.Instrument.InstrumentType;

        public override void OnInitializing()
        {
            base.OnInitializing();

            var itemsValues = JsonConvert.DeserializeObject<Dictionary<int, string>>(_instrumentData);
            var newItems = Items.ToList();
            newItems.AddRange(ItemHelpers.LoadItems(TocHoneywellClient.TurboMonitor, itemsValues));
            newItems.RemoveAll(value => value.Metadata == null);
            Items = newItems;

            if (!string.IsNullOrEmpty(_afterTestData))
            {
                var afterItemValues = JsonConvert.DeserializeObject<Dictionary<int, string>>(_afterTestData);
                var items = ItemHelpers.LoadItems(VerificationTest.Instrument.InstrumentType, afterItemValues).ToList();
                items.AddRange(ItemHelpers.LoadItems(TocHoneywellClient.TurboMonitor, afterItemValues));
                items.RemoveAll(value => value.Metadata == null);
                AfterTestItems = items;
            }
        }
    }
}
