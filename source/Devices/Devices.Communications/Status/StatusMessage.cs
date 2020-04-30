using System.Collections.Generic;
using System.Linq;
using Devices.Communications.IO;
using Devices.Core.Items;
using Microsoft.Extensions.Logging;

namespace Devices.Communications.Status
{
    public enum CommDirectionEnum
    {
        Sent,
        Received
    }

    public static class Messages
    {
        public static CommunicationsMessage DataRecieved(string data) =>
            new CommunicationsMessage(CommDirectionEnum.Received, data);

        public static CommunicationsMessage DataSent(string data) =>
            new CommunicationsMessage(CommDirectionEnum.Sent, data);

        public static StatusMessage Debug(string message) => new StatusMessage(LogLevel.Debug, message);
        public static StatusMessage Error(string message) => new StatusMessage(LogLevel.Error, message);
        public static StatusMessage Info(string message) => new StatusMessage(LogLevel.Information, message);

        public static void MessageConnection(this CommunicationsClient client, int attempt, int maxAttempts) =>
            client.PublishMessage(client.ConnectionStatus(attempt, maxAttempts));

        public static void MessageDebug(this CommunicationsClient client, string message) =>
            client.PublishMessage(Debug(message));

        public static void MessageInfo(this CommunicationsClient client, string message) =>
            client.PublishMessage(Info(message));

        public static void MessageItemReadStatus(this CommunicationsClient client, ICollection<ItemMetadata> items, ICollection<ItemValue> itemValues) =>
            client.PublishMessage(new ItemReadStatusMessage(LogLevel.Debug, items.ToList(), itemValues.ToList()));

        public static StatusMessage Warning(string message) => new StatusMessage(LogLevel.Warning, message);

        private static StatusMessage ConnectionStatus(this CommunicationsClient client, int attempt, int maxAttempts) =>
            new StatusMessage(LogLevel.Debug,
                titleMessage: $"Connecting to {client.DeviceType.Name} on {client.CommPort.Name}.",
                message: $"Attempt {attempt} of {maxAttempts}");

        //public static void PublishStatus(this CommunicationsClient commClient, StatusMessage message)
        //{
        //    commClient.StatusMessageObservable.
        //}
    }

    public class StatusMessage
    {
        public StatusMessage(LogLevel logLevel, string message = null, string titleMessage = null)
        {
            LogLevel = logLevel;
            Message = message;
            TitleMessage = titleMessage;
        }

        public LogLevel LogLevel { get; protected set; }

        public string TitleMessage { get; set; }

        protected string Message { get; set; }

        public override string ToString() => $"{Message}";
    }

    public class ConnectionStatusMessage : StatusMessage
    {
        protected ConnectionStatusMessage(LogLevel logLevel) : base(logLevel, "Connection to")
        {
        }

        public ConnectionStatusMessage(LogLevel logLevel, string message, string titleMessage = null) : base(logLevel,
            message, titleMessage)
        {
        }
    }

    public class CommunicationsMessage : StatusMessage
    {
        public CommunicationsMessage(CommDirectionEnum direction, string data) : base(LogLevel.Debug)
        {
            Direction = direction;
            Message = $"{ControlCharacters.Prettify(data)}";
        }

        public CommDirectionEnum Direction { get; }

        public override string ToString() => $"[{(Direction == CommDirectionEnum.Sent ? 'S' : 'R')}] >> {Message}";
    }

    public class ItemReadStatusMessage : StatusMessage
    {
        public ItemReadStatusMessage(LogLevel logLevel, ICollection<ItemMetadata> itemMetadata,
            ICollection<ItemValue> itemValues)
            : base(logLevel, titleMessage: "Downloading items")
        {
            //ItemMetadata = itemMetadata;
            //ItemValues = itemValues;

            TotalCount = itemMetadata.Count;
            ReadCount = itemValues.Count;
        }

        public ICollection<ItemMetadata> ItemMetadata { get; }
        public ICollection<ItemValue> ItemValues { get; }

        public int ReadCount { get; set; }

        public int TotalCount { get; set; }

        public override string ToString() => $"{ReadCount} of {TotalCount} items";
    }
}