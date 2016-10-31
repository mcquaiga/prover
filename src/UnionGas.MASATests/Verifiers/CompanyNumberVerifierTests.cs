using System.Collections.Generic;
using System.Management.Instrumentation;
using Moq;
using NUnit.Framework;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Models.Instruments;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Verifiers;

namespace UnionGas.MASATests.Verifiers
{
    [TestFixture()]
    public class CompanyNumberVerifierTests
    {
        private Mock<EvcCommunicationClient> commClient;

        [SetUp]
        protected void Setup()
        {
            var itemMetadata = new ItemMetadata()
            {
                Code = ItemCodes.SiteInfo.CompanyNumber,
            };

            var itemValue = new ItemValue(itemMetadata, "0000000");
            Instrument = new Instrument
            {
                Items = new List<ItemValue>()
                {
                    itemValue
                }
            };

            commClient = new Mock<EvcCommunicationClient> { CallBase = true };
            WebService = new Mock<DCRWebServiceSoap>();
        }

        public Mock<DCRWebServiceSoap> WebService { get; set; }

        public Instrument Instrument { get; set; }

        [Test()]
        public void VerifySuccessTest()
        {
            //WebService.Setup(ws => ws.GetValidatedEvcDeviceByInventoryCode()).Returns()
            //verifier.Setup(x => x.Verify(commClient, Instrument))
            //Assert.IsTrue(true);
        }
    }
}