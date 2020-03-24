namespace Devices.Core.Items.Descriptions
{
    /// <summary>
    ///     Defines the <see cref="ItemDescription" />
    /// </summary>
    public class ItemDescription : IHaveOneId
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the Description
        /// </summary>
        public string Description { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The ToString
        /// </summary>
        /// <returns>The <see cref="string" /></returns>
        public override string ToString()
        {
            return $"{Description}";
        }

        public virtual object GetValue()
        {
            return Description;
        }

        #endregion
    }

    public class ItemDescriptionWithNumericValue : ItemDescription
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the NumericValue
        /// </summary>
        public decimal NumericValue { get; set; }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return $"{Description} - Numeric Value: {NumericValue}";
        }

        public override object GetValue()
        {
            return NumericValue;
        }

        #endregion
    }
}