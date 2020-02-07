using System;
using Devices.Core.Items.Attributes;

namespace Devices.Honeywell.Core.Items.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class JoinLowResHighResValueAttribute : ItemInfoAttribute
    {
        public Func<decimal, decimal, decimal> ResultSelector { get; }
        public int SecondNumber { get; }

        public JoinLowResHighResValueAttribute(int firstItem, int secondItem)
        {
            Number = firstItem;
            SecondNumber = secondItem;
            ResultSelector = (x, y) => ItemValueParses.JoinLowResHighResReading(x, y);
        }
    }
}