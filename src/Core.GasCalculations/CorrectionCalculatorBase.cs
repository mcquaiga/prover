﻿using Devices.Core.Interfaces;
using Shared.Extensions;

namespace Core.GasCalculations
{
    public interface ICorrectionCalculator
    {
        /// <summary>
        /// Calculated value
        /// </summary>

        decimal CalculateFactor();
    }

    public abstract class CorrectionCalculatorBase<T> : ICorrectionCalculator where T : IItemGroup
    {
        protected  CorrectionCalculatorBase(){ }
        protected CorrectionCalculatorBase(T items)
        {
            Items = items;
            
        }

        #region Public Properties

        public T Items { get; protected set; }


        public abstract decimal CalculateFactor();


        #endregion
    }
}