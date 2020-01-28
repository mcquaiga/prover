using System;
using Devices.Honeywell.Core.Items;

namespace Devices.Honeywell.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ItemInfoAttribute : Attribute
    {
        public int Number { get; set; }

        public ItemInfoAttribute()
        {
        }

        public ItemInfoAttribute(int number)
        {
            Number = number;
        }
    }

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