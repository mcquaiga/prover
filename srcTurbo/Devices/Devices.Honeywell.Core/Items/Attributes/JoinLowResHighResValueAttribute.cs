using System;

namespace Devices.Honeywell.Core.Items.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class JoinLowResHighResValueAttribute : Attribute
    {
        public int FirstItem { get; }

        public Func<decimal, decimal, decimal> ResultSelector { get; }
        public int SecondItem { get; }

        public JoinLowResHighResValueAttribute(int firstItem, int secondItem)
        {
            FirstItem = firstItem;
            SecondItem = secondItem;
            ResultSelector = (x, y) => ItemValueParses.JoinLowResHighResReading(x, y);
        }
    }
}