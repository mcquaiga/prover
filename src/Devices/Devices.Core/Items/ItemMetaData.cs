using Devices.Core.Items.Descriptions;

namespace Devices.Core.Items
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="IHaveManyId"/>
    /// </summary>
    public interface IHaveManyId
    {
        /// <summary>
        /// Gets or sets the Ids
        /// </summary>
        int[] Ids { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="IHaveOneId"/>
    /// </summary>
    public interface IHaveOneId
    {
        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        int Id { get; set; }
    }
    

    /// <summary>
    /// Defines the <see cref="ItemMetadata"/>
    /// </summary>
    public class ItemMetadata : IEqualityComparer<ItemMetadata>
    {
        private readonly List<ItemDescription> _itemDescriptions = new List<ItemDescription>();

        public ItemMetadata()
        {
        }

        [JsonConstructor]
        public ItemMetadata(ICollection<ItemDescription> itemDescriptions = null)
        {
            _itemDescriptions = itemDescriptions?.ToList() ?? new List<ItemDescription>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether CanReset
        /// </summary>
        public bool CanReset { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether CanVerify
        /// </summary>
        public bool CanVerify { get; set; } = true;

        /// <summary>
        /// Gets or sets the Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the IsAlarm
        /// </summary>
        public bool? IsAlarm { get; set; } = false;

        /// <summary>
        /// Gets or sets the IsFrequencyTest
        /// </summary>
        public bool? IsFrequencyTest { get; set; } = false;

        /// <summary>
        /// Gets or sets the IsLiveReadPressure
        /// </summary>
        public bool? IsLiveReadPressure { get; set; }

        /// <summary>
        /// Gets or sets the IsLiveReadTemperature
        /// </summary>
        public bool? IsLiveReadTemperature { get; set; }

        /// <summary>
        /// Gets or sets the IsPressure
        /// </summary>
        public bool? IsPressure { get; set; } = false;

        /// <summary>
        /// Gets or sets the IsPressureTest
        /// </summary>
        public bool? IsPressureTest { get; set; } = false;

        /// <summary>
        /// Gets or sets the IsSuperFactor
        /// </summary>
        public bool? IsSuperFactor { get; set; } = false;

        /// <summary>
        /// Gets or sets the IsTemperature
        /// </summary>
        public bool? IsTemperature { get; set; } = false;

        /// <summary>
        /// Gets or sets the IsTemperatureTest
        /// </summary>
        public bool? IsTemperatureTest { get; set; } = false;

        /// <summary>
        /// Gets or sets the IsVolume
        /// </summary>
        public bool? IsVolume { get; set; } = false;

        /// <summary>
        /// Gets or sets the IsVolumeTest
        /// </summary>
        public bool? IsVolumeTest { get; set; } = false;

        /// <summary>
        /// Gets or sets the ItemDescriptions
        /// </summary>
        public virtual ICollection<ItemDescription> ItemDescriptions => _itemDescriptions.AsReadOnly();

        /// <summary>
        /// Gets or sets the Number
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets the ShortDescription
        /// </summary>
        public string ShortDescription { get; set; }

        public bool Equals(ItemMetadata x, ItemMetadata y)
        {
            return x.Number == y.Number;
        }

        public int GetHashCode(ItemMetadata obj)
        {
            return obj.GetHashCode();
        }

        /// <summary>
        /// The GetItemDescription
        /// </summary>
        /// <param name="value">The rawValue <see cref="string"/></param>
        /// <returns>The <see cref="ItemDescriptionBasic"/></returns>
        public virtual ItemDescription GetItemDescription(object value)
        {
            if (ItemDescriptions != null && ItemDescriptions.Any())
            {
                if (!int.TryParse(value.ToString(), out var intValue))
                    return ItemDescriptions.FirstOrDefault(x => x.Description.Contains(value.ToString()));

                var result = ItemDescriptions.FirstOrDefault(x => (x as IHaveManyId)?.Ids.Contains(intValue) ?? false);

                if (result == null)
                    result = ItemDescriptions.FirstOrDefault(x => (x as IHaveOneId)?.Id == intValue);

                return result;
            }

            return null;
        }

        /// <summary>
        /// The ToString
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public override string ToString()
        {
            return $"{Number} - {Description}";
        }
    }

    
}