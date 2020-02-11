namespace Domain.Interfaces
{
    public interface ICompareTestResults<T> where T : struct
    {
        #region Public Properties

        /// <summary>
        ///     Value produced by the EVC
        /// </summary>
        //T ActualValue { get; }

        ///// <summary>
        /////     Calculated value
        ///// </summary>
        //T ExpectedValue { get; }

        //T Variance { get; }

        T Variance(T expectedValue, T actualValue);

        #endregion
    }
}