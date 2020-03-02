using System.Collections.Generic;
using System.Linq;
using Devices.Core.Items;
using Newtonsoft.Json;
using static Tests.Honeywell.Core.ItemTestFixtures;

namespace Tests.Honeywell.Core
{
    internal static class ItemFileMocks
    {
    }

    internal static class ItemTestData
    {
        #region Properties

        public static string MeterTypeDescriptionsJson => @"{
                'type': 'Devices.Core.Items.Descriptions.RotaryMeterTypeDescription, Devices.Core, Version = 1.0.0.0, Culture = neutral, PublicKeyToken = null',
                  'ItemDescriptions': [
                    {
                      'description': 'No Meter Type',
                      'UnCorPulsesX10': '0',
                      'UnCorPulsesX100': '0',
                      'MeterDisplacement': '0.00',
                      'MountType': 'LMMA',
                      'ids': [ '0' ]
                    },
                    {
                      'description': 'Roots 2M',
                      'UnCorPulsesX10': '12',
                      'UnCorPulsesX100': '2',
                      'MeterDisplacement': '0.0110558',
                      'MountType': 'LMMA',
                      'ids': [
                        '45',
                        '81'
                      ]
                    }
                ]
            }";

        public static string MeterTypeJson => @"[
                {
                    'canReset': 'false',
                    'canVerify': 'false',
                    'code': 'METER_TYPE',
                    'description': 'Meter Type',
                    'isVolume': 'true',
                    'ItemDescriptions': [
                      {
                        'definitionPath': 'Item_MeterType'
                      }
                    ],
                    'number': '432',
                    'shortDescription': 'Meter Type'
                  }
            ]";

        internal static ItemTestFixtures OneItem =>
                           new ItemTestFixtures(new ItemDefinitionTest()
                           {
                               Number = 0,
                               Code = "COR_VOL",
                               ShortDescription = "Corrected Vol",
                               Description = "Corrected Volume",
                               IsVolume = true,
                               IsVolumeTest = true
                           });

        #endregion
    }

    internal class ItemTestFixtures
    {
        #region Properties

        public ICollection<ItemDefinitionTest> Items { get; private set; }

        public override bool Equals(object obj)
        {
            if (obj.GetType().IsAssignableFrom(typeof(ICollection<ItemMetadata>)))
                return base.Equals(obj);

            foreach (var i in (IEnumerable<ItemMetadata>)obj)
            {
                if (!Items.Any(exp => exp.Equals(i)))
                {
                    return false;
                }
            }

            return true;
        }

        public ItemDefinitionTest GetFirst() => Items.FirstOrDefault();

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.None,
               new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Serialize });
        }

        internal ItemTestFixtures(ItemDefinitionTest item)
        {
            Items = new List<ItemDefinitionTest>() { item };
        }

        internal class ItemDefinitionTest : ItemMetadata
        {
            #region Methods

            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }

            #endregion
        }

        #endregion
    }
}