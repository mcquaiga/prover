using System;
using System.Collections.Generic;
using AutoMapper;
using Newtonsoft.Json;
using NUnit.Framework;
using Prover.InstrumentProtocol.Core.Models.Instrument;
using Prover.InstrumentProtocol.Honeywell.Domain.Instrument.MiniMax;
using Prover.InstrumentProtocol.Honeywell.Factories;
using Prover.Shared.DTO.Instrument;

namespace Prover.CommProtocol.MiHoneywell.Tests.Domain.Instrument
{
    [TestFixture()]
    public class MiniMaxInstrumentTests
    {
        private string _ptzItemFile =
            "{\"0\":\" 0000000\",\"2\":\" 0000000\",\"5\":\" 0000000\",\"6\":\" 0000000\",\"8\":\"    0.00\",\"10\":\"99999.98\",\"11\":\"   -1.00\",\"12\":\"  0.0000\",\"13\":\" 14.7300\",\"14\":\" 14.4000\",\"26\":\"  -40.00\",\"27\":\"  -35.00\",\"28\":\"  145.00\",\"34\":\"   60.00\",\"35\":\"  0.0893\",\"44\":\"  0.9778\",\"45\":\"  1.2383\",\"47\":\"  1.0000\",\"53\":\"  0.5300\",\"54\":\"  1.7500\",\"55\":\"  0.7230\",\"56\":\"  2.0000\",\"57\":\"  2.0000\",\"62\":\"01084240\",\"87\":\"       0\",\"89\":\"       0\",\"90\":\"       7\",\"92\":\"       4\",\"93\":\"       2\",\"94\":\"       0\",\"98\":\"      14\",\"109\":\"       0\",\"110\":\"       0\",\"111\":\"       0\",\"112\":\"       0\",\"113\":\"  0.0000\",\"122\":\"  2.9032\",\"137\":\"  100.00\",\"140\":\" 0000000\",\"141\":\"       0\",\"142\":\" 1000.00\",\"147\":\"       0\",\"200\":\"01084240\",\"201\":\"02547086\",\"432\":\"       2\",\"439\":\".0222222\",\"892\":\"  0.0000\"}";
        private string _invalidItemFile = "{\"2000\":\"00003922\"}";

        [SetUp]
        public void Setup()
        {
        }

        [Test()]
        public void Test_Create_MiniMaxFactory_With_Valid_Item_Dictionary()
        {
            var itemsData = GetItemDictionary(_ptzItemFile);
            var instrument = CreateMiniMax(itemsData);
            Assert.AreEqual(itemsData, instrument.ItemData);
        }

        [Test]
        public void Test_Mapping_Instrument_to_Dto()
        {
            var itemsData = GetItemDictionary(_ptzItemFile);
            var instrument = CreateMiniMax(itemsData);

            var dto = Mapper.Map<InstrumentDto>(instrument);
            Assert.IsNotNull(dto);

            var domain = Mapper.Map<IInstrument>(dto);
            Assert.IsNotNull(domain);
        }

        [Test()]
        public void Test_Create_MiniMaxFactory_With_Invalid_Item_Dictionary()
        {
            //var factory = new MiniMaxFactory();
            //var itemsData = GetItemDictionary(_invalidItemFile);
            //var instrument = factory.Create(itemsData).Result;

            //Assert.AreNotEqual(itemsData, instrument.ItemData);
            //Assert.IsEmpty(instrument.ItemData);
            //Assert.Catch<Exception>(() =>
            //{
            //    var p = instrument.PressureItems;
            //});
        }

        private IInstrument CreateMiniMax(Dictionary<string, string> itemsData)
        {
            //var factory = new MiniMaxFactory();
            return null;
            //return factory.Create(itemsData).Result;
        }

        private Dictionary<string, string> GetItemDictionary(string dictionaryJson)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(dictionaryJson);
        }
    }
}