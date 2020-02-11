using System;
using Core.GasCalculations;
using Devices.Core.Interfaces;
using Domain.EvcVerifications.CorrectionTests;

namespace Application.ViewModels
{
    public abstract class CorrectionTestViewModel<T> where T : IItemGroup
    {
        protected CorrectionFactory.CorrectionTestCalculatorDecorator FactorTestCalculatorDecorator;
        protected CorrectionTestViewModel(T items)
        {
            Items = items;
        }
        public T Items { get; set; }

        public abstract Func<ICorrectionCalculator> CalculatorFactory { get; }

        public CorrectionTest FactorTest => FactorTestCalculatorDecorator;

        public virtual void Update(T items)
        {
            Items = items;
          
            FactorTestCalculatorDecorator.Calculator = CalculatorFactory.Invoke();
            FactorTestCalculatorDecorator.CalculateFactors();
        }
    }
}