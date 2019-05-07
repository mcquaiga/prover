using System;

namespace Devices.Honeywell.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ItemInfoAttribute : Attribute
    {
        public ItemInfoAttribute()
        {
        }

        public ItemInfoAttribute(int number)
        {
            Number = number;
        }

        public int Number { get; set; }
    }
}