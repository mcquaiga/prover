using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Prover.Shared.Events;

namespace Prover.Application.Verifications.Events.Tests
{
	[TestClass()]
	public class VerificationEventTests
	{
		private static EventHub<string, string> _event = new EventHub<string, string>();

		[TestMethod()]
		public async Task PublishTest()
		{
			_event.Subscribe(x => x.SetOutput("Bar"));

			var result = await _event.Publish("FooEvent");

			Assert.IsTrue(result == "Bar");
		}
	}
}