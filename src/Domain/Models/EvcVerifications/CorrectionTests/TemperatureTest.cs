using Devices.Core;
using Devices.Core.Items.ItemGroups;

namespace Domain.Models.EvcVerifications.CorrectionTests
{
    /// <summary>
    ///     Defines the <see cref="TemperatureTest" />
    /// </summary>
    public class TemperatureTest : CorrectionBase<ITemperatureItems>
    {
        public TemperatureTest(ITemperatureItems items, decimal actual, decimal expected, decimal gauge) : base(items, actual, expected)
        {
            Gauge = gauge;
        }

        #region Public Properties
        /// <summary>
        ///     Gets or sets the Gauge
        /// </summary>
        public decimal Gauge { get; set; }

        /// <summary>
        ///     Gets the PassTolerance
        /// </summary>
        public override decimal PassTolerance => Global.TEMP_ERROR_TOLERANCE;

        #endregion
    }
}