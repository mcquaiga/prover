using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Devices.Communications;
using Devices.Communications.IO;
using Devices.Communications.Status;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Honeywell.Comm.Messaging.Requests;
using Devices.Honeywell.Comm.Messaging.Responses;
using Devices.Honeywell.Comm.Messaging.Responses.Codes;
using Devices.Honeywell.Core;
using Devices.Honeywell.Core.Items;

namespace Devices.Honeywell.Comm.CommClients
{
    public abstract class HoneywellClientBase<TDevice> : CommunicationsClient
        where TDevice : HoneywellDeviceType
    {
        protected HoneywellClientBase(ICommPort commPort, TDevice deviceType) : base(commPort, deviceType)
        {
        }

        #region Public Methods

        public override async Task<IEnumerable<ItemValue>> GetItemValuesAsync(IEnumerable<ItemMetadata> itemNumbers)
        {
            var itemDetails = itemNumbers.ToList();
            var items = itemDetails.GetAllItemNumbers().ToArray();
            var results = new List<ItemValue>();

            if (items.Count() == 1)
            {
                var value = await GetItemValue(itemDetails[0]);
                results.Add(value);
                return results;
            }

            items = items.OrderBy(x => x).ToArray();

            var y = 0;

            while (true)
            {
                var set = items.Skip(y).Take(15).ToList();

                if (!set.Any())
                    break;

                var response = await ExecuteCommandAsync(Commands.ReadGroup(set));
                foreach (var item in response.ItemValues)
                {
                    var metadata = itemDetails.FirstOrDefault(x => x.Number == item.Key);
                    results.Add(ItemValue.Create(metadata, item.Value));
                }

                y += 15;
            }

            return results;
        }

        #endregion

        #region Protected

        //public override async Task<bool> SetItemValue(string itemCode, long value)
        //    => await SetItemValue(ItemDetails.GetItem(itemCode), value);

        protected override async Task EstablishConnectionAsync(CancellationToken ct)
        {
            if (await WakeUpInstrument(ct))
            {
                var response = 
                        await ExecuteCommandAsync(
                            Commands.SignOn((TDevice)DeviceType)
                            );

                if (HandleResponseMessage<HoneywellDeviceType>(response))
                {
                    IsConnected = true;
                    PublishMessage(Messages.Info($"[{CommPort.Name}] Connected to {DeviceType.Name}!"));
                }
                else
                {
                    IsConnected = false;
                }
            }
        }

        public override async Task<bool> SetItemValue(int itemNumber, string value)
        {
            var result = await ExecuteCommandAsync(Commands.WriteItem(itemNumber, value));

            return result.IsSuccess;
        }

        public override async Task<bool> SetItemValue(int itemNumber, decimal value)
            => await SetItemValue(itemNumber, value.ToString(CultureInfo.CurrentCulture));

        public override async Task<bool> SetItemValue(int itemNumber, long value)
            => await SetItemValue(itemNumber, value.ToString());

        protected async Task<bool> WakeUpInstrument(CancellationToken ct)
        {
            await ExecuteCommandAsync(Commands.WakeupOne());
            await Task.Delay(200, ct);

            try
            {
                var response = await ExecuteCommandAsync(Commands.WakeupTwo());

                if (response.IsSuccess || response.ResponseCode == ResponseCode.InvalidEnquiryError) return true;
            }
            catch (Exception ex)
            {
                if (!(ex is TimeoutException))
                {
                    await Task.Delay(200, ct);
                    await ExecuteCommandAsync(Commands.OkayToSend());
                }

                throw;
            }

            return false;
        }

        #endregion

        #region Private

        private bool HandleResponseMessage<T>(StatusResponseMessage response)
        {
            if (response.IsSuccess)
                return true;

            var responseType = Responses.Get(response.ResponseCode);

            //responseType.TryRecover(this);

            if (responseType.ThrowsException)
                throw responseType.RaiseException(response);

            return false;
        }

        #endregion

        #region Public

        #region Properties

        public override bool IsConnected { get; protected set; }

        #endregion

        #region Methods

        public override async Task<ItemValue> GetItemValue(ItemMetadata itemNumber)
        {
            var response = await ExecuteCommandAsync(Commands.ReadItem(itemNumber.Number));
            return ItemValue.Create(itemNumber, response.RawValue);
        }

        public override async Task<ItemValue> LiveReadItemValue(int itemNumber)
        {
            var itemDetails = DeviceType.Items.GetItem(itemNumber);
            if (itemDetails == null)
                throw new Exception($"Item with #{itemNumber} does not exist in metadata.");

            var response = await ExecuteCommandAsync(Commands.LiveReadItem(itemNumber));
            return ItemValue.Create(itemDetails, response.RawValue);
        }

        public override async Task LiveReadItemValue(int itemNumber, IObserver<ItemValue> updates,
            CancellationToken ct)
        {
            do
            {
                var i = await LiveReadItemValue(itemNumber);
                updates.OnNext(i);
                await Task.Delay(500, ct);
            } while (!ct.IsCancellationRequested);
        }

        protected override async Task TryDisconnect()
        {
            if (IsConnected)
            {
                var response = await ExecuteCommandAsync(Commands.SignOffCommand());

                if (response.IsSuccess)
                    IsConnected = false;
            }
        }

        #endregion

        #endregion
    }
}