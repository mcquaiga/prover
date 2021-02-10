using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.Common.Models.Instrument;
using Prover.CommProtocol.MiHoneywell.Messaging.Requests;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.CommProtocol.MiHoneywell.CommClients {
	public class HoneywellClient : EvcCommunicationClient {
		protected Task LoadItemsTask { get; private set; }

		public HoneywellClient(ICommPort commPort, IEvcDevice instrumentType, ISubject<string> statusSubject) : base(commPort, instrumentType, statusSubject) {
		}

		public override bool IsConnected { get; protected set; }

		protected override async Task ConnectToInstrument(CancellationToken ct, string accessCode = null) {
			await Task.Run(async () => {
				await Task.Delay(200);
				if (await WakeUpInstrument()) {
					var response = await ExecuteCommand(Commands.SignOn(InstrumentType, accessCode));

					if (response.IsSuccess) {
						IsConnected = true;
						Log.Info($"[{CommPort.Name}] Connected to {InstrumentType.Name}!");
					}
					else {
						IsConnected = false;

						if (response.ResponseCode == ResponseCode.FramingError)
							await CommPort.Close();

						throw new Exception($"Error response {response.ResponseCode}");
					}
				}
			}, ct);

		}

		public override async Task Disconnect() {
			if (IsConnected) {
				var response = await ExecuteCommand(Commands.SignOffCommand());

				if (response.IsSuccess)
					IsConnected = false;
			}
		}

		public override async Task<ItemValue> GetItemValue(ItemMetadata itemNumber) {
			var itemDetails = itemNumber;
			var response = await ExecuteCommand(Commands.ReadItem(itemNumber.Number));
			return new ItemValue(itemDetails, response.RawValue);
		}

		public override async Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<ItemMetadata> itemNumbers) {
			var itemDetails = itemNumbers.ToList();
			var items = itemDetails.GetAllItemNumbers().ToArray();
			var results = new List<ItemValue>();

			if (items.Count() == 1) {
				var value = await GetItemValue(itemDetails[0]);
				results.Add(value);
				return results;
			}

			items = items.OrderBy(x => x).ToArray();

			var y = 0;

			while (true) {
				var set = items.Skip(y).Take(15).ToList();

				if (!set.Any())
					break;

				var response = await ExecuteCommand(Commands.ReadGroup(set));
				foreach (var item in response.ItemValues) {
					var metadata = itemDetails.FirstOrDefault(x => x.Number == item.Key);
					results.Add(new ItemValue(metadata, item.Value));
				}

				y += 15;
			}

			return results;
		}

		public override Task<IFrequencyTestItems> GetFrequencyItems() {
			throw new NotImplementedException();
		}

		public override async Task<bool> SetItemValue(int itemNumber, string value) {
			var result = await ExecuteCommand(Commands.WriteItem(itemNumber, value));

			return result.IsSuccess;
		}

		public override async Task<bool> SetItemValue(int itemNumber, decimal value)
			=> await SetItemValue(itemNumber, value.ToString(CultureInfo.InvariantCulture));

		public override async Task<bool> SetItemValue(int itemNumber, long value)
			=> await SetItemValue(itemNumber, value.ToString());

		public override async Task<bool> SetItemValue(string itemCode, long value)
			=> await SetItemValue(ItemDetails.GetItem(itemCode), value);

		public override async Task<ItemValue> LiveReadItemValue(int itemNumber) {
			var itemDetails = ItemDetails.GetItem(itemNumber);
			var response = await ExecuteCommand(Commands.LiveReadItem(itemNumber));
			return new ItemValue(itemDetails, response.RawValue);
		}

		private async Task<bool> WakeUpInstrument() {
			await ExecuteCommand(Commands.WakeupOne());
			await Task.Delay(250);

			try {
				var response = await ExecuteCommand(Commands.WakeupTwo());
				await Task.Delay(200);
				if (response.IsSuccess || (response.ResponseCode == ResponseCode.InvalidEnquiryError)) {
					return true;
				}
			}
			catch (Exception) {
				Thread.Sleep(200);
				await ExecuteCommand(Commands.OkayToSend());
			}

			return false;
		}
	}
}