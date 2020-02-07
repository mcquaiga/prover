using System;

namespace Devices.Core.Items.Attributes
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
}