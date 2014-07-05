using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prover.Core.Models.Instruments;

namespace Prover.Core.Tests
{
    [TestClass]
    public class Items
    {
        [TestMethod]
        public void LoadItems()
        {
            var mylist = Item.LoadItems(InstrumentType.MiniMax);

            foreach (var i in mylist)
            {
                Console.WriteLine(i.ShortDescription);
            }
        }


    }
}
