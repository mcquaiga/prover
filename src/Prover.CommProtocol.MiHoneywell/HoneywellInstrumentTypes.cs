﻿using System.Collections.Generic;
using System.Linq;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.MiHoneywell.Items;

namespace Prover.CommProtocol.MiHoneywell
{
    public static class HoneywellInstrumentTypes
    {
        public static EvcDevice GetByName(string name)
        {
            var all = GetAll();

            return all.ToList().FirstOrDefault(i => i.Name == name);
        }

        public static EvcDevice GetById(int id)
        {
            var all = GetAll();

            return all.ToList().FirstOrDefault(i => i.Id == id);
        }

        public static IEnumerable<EvcDevice> GetAll()
        {
            var allTask = ItemHelpers.GetInstrumentDefinitions().ConfigureAwait(false);
            return allTask.GetAwaiter().GetResult()
                .ToList()
                .OrderBy(i => i.Name);
        }
    }
}
/*
namespace Prover.CommProtocol.MiHoneywell
{
    using Prover.CommProtocol.Common;
    using Prover.CommProtocol.Common.Items;
    using Prover.CommProtocol.MiHoneywell.CommClients;
    using Prover.CommProtocol.MiHoneywell.Items;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="Instruments" />
    /// </summary>
    public static class Instruments
    {
        public static void LoadInstrumentTypes()
        {            
            foreach(var i in GetAll())
            {
                i.LoadItemsInformation();
            }

            TurboMonitor.LoadItemsInformation();
        }

        #region Fields

        /// <summary>
        /// Defines the MiniAt
        /// </summary>
        public static InstrumentType MiniAt = new MiInstrumentType
        {
            Id = 3,
            AccessCode = 3,
            Name = "Mini-AT",
            ItemFilePath = "MiniATItems.xml",
            ClientFactory = port => new HoneywellClient(port, Instruments.MiniAt)
        };

        /// <summary>
        /// Defines the MiniMax
        /// </summary>
        public static InstrumentType MiniMax = new MiInstrumentType
        {
            Id = 4,
            AccessCode = 4,
            Name = "Mini-Max",
            ItemFilePath = "MiniMaxItems.xml",
            ClientFactory = port => new HoneywellClient(port, Instruments.MiniMax)
        };

        /// <summary>
        /// Defines the Toc
        /// </summary>
        public static InstrumentType Toc = new MiInstrumentType
        {
            Id = 33,
            AccessCode = 3,
            Name = "TOC",
            ItemFilePath = "TOCItems.xml",
            ClientFactory = port => new TocHoneywellClient(port, Toc)
        };

        /// <summary>
        /// Defines the TurboMonitor
        /// </summary>
        public static InstrumentType TurboMonitor = new MiInstrumentType()
        {
            Id = 6,
            AccessCode = 6,
            Name = "Turbo Monitor",
            ItemFilePath = "TurboMonitorItems.xml"
        };

        #endregion

        #region Methods

        /// <summary>
        /// The GetAll
        /// </summary>
        /// <returns>The <see cref="IEnumerable{InstrumentType}"/></returns>
        public static IEnumerable<InstrumentType> GetAll()
        {
            return new List<InstrumentType>
            {
                MiniAt,
                MiniMax,
                Toc
            };
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="MiInstrumentType" />
    /// </summary>
    public class MiInstrumentType : InstrumentType
    {
        #region Fields

        /// <summary>
        /// Defines the _itemsInformation
        /// </summary>
        private readonly HashSet<ItemMetadata> _itemsInformation = new HashSet<ItemMetadata>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ItemsInformation
        /// </summary>
        public override HashSet<ItemMetadata> Items
        {
            get
            {
                if (!_itemsInformation.Any())
                {
                    LoadItemsInformation();
                }

                return _itemsInformation;
            }
        }

        public override void LoadItemsInformation()
        {
            _itemsInformation.Clear();
            _itemsInformation.UnionWith(ItemHelpers.LoadItems(this));        
        }

        #endregion
    }
}
*/