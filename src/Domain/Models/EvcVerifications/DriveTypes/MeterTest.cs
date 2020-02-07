using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Domain.Calculators.Helpers;
using Domain.Interfaces;

namespace Domain.Models.EvcVerifications.DriveTypes
{
    public class MeterTest : IAssertPassFail
    {
        public decimal? EvcMeterDisplacement { get; set; }

        public decimal MeterDisplacement { get; set; }

        public decimal MeterDisplacementPercentError { get; set; }

        public MeterIndexItemDescription MeterIndex { get; set; }

        public string MeterType { get; set; }

        //public decimal? EvcMeterDisplacement => _instrument.Items.GetItem(439).NumericValue;

        //public decimal MeterDisplacement
        //{
        //    get
        //    {
        //        if (MeterIndex != null)
        //            return MeterIndex.MeterDisplacement.Value;

        //        return 0;
        //    }
        //}

        //public bool MeterDisplacementHasPassed => MeterDisplacementPercentError.IsBetween(Global.METER_DIS_ERROR_THRESHOLD);

        //public decimal MeterDisplacementPercentError
        //{
        //    get
        //    {
        //        if (MeterDisplacement != 0)
        //            return Math.Round((decimal)((EvcMeterDisplacement - MeterDisplacement) / MeterDisplacement * 100),
        //                2);
        //        return 0;
        //    }
        //}

        //public MeterIndexItemDescription MeterIndex
        //    => (MeterIndexItemDescription)_instrument.Items.GetItem(432).Metadata.ItemDescriptions
        //        .FirstOrDefault(x => (x as IHaveManyId).Ids.Contains(MeterTypeId));

        //public string MeterType
        //    => !string.IsNullOrEmpty(MeterTypeDescription)
        //        ? MeterTypeDescription
        //        : _instrument.Items.GetItem(432).Description;

        //public string MeterTypeDescription => MeterIndex.Description;
        //public int MeterTypeId => (int)_instrument.Items.GetItem(432).NumericValue;

        //private readonly EvcVerification _instrument;
        public bool Passed => MeterDisplacementPercentError.IsBetween(Global.METER_DIS_ERROR_THRESHOLD);

        public MeterTest(IDeviceWithValues device)
        {
            if (MeterIndex == null)
                throw new KeyNotFoundException("Could not find a meter type that match the instruments value in item 432.");
        }
    }
}