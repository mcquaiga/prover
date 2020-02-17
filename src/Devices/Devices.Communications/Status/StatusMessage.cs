using System.Collections.Generic;
using Devices.Communications.IO;
using Devices.Core.Items;
using DynamicData.Diagnostics;
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
        public static StatusMessage Debug(string message) => new StatusMessage(LogLevel.Debug, message);
        public static StatusMessage Warning(string message) => new StatusMessage(LogLevel.Warning, message);
        public static StatusMessage Error(string message) => new StatusMessage(LogLevel.Error, message);
        public static StatusMessage Info(string message) => new StatusMessage(LogLevel.Information, message);

        public static ConnectionStatusMessage Connection => new ConnectionStatusMessage(LogLevel.Information, "");

        public static CommunicationsMessage DataRecieved(string data) => new CommunicationsMessage(CommDirectionEnum.Received, data);
        public static CommunicationsMessage DataSent(string data) => new CommunicationsMessage(CommDirectionEnum.Sent, data);

        //public static void PublishStatus(this CommunicationsClient commClient, StatusMessage message)
        //{
        //    commClient.StatusMessageObservable.
        //}
    }

    public class StatusMessage
    {
        protected StatusMessage(LogLevel logLevel, string titleMessage = null)
        {
            LogLevel = logLevel;
            TitleMessage = titleMessage;
            Message = string.Empty;
        }

        public StatusMessage(LogLevel logLevel, string message, string titleMessage = null)
        {
            LogLevel = logLevel;
            Message = message;
            TitleMessage = titleMessage;
        }

        public LogLevel LogLevel { get; protected set; }

        public string TitleMessage { get; set; }

        protected string Message { get; set; }

        public override string ToString()
        {
            return $"{Message}";
        }
    }

    public class ConnectionStatusMessage : StatusMessage
    {
        protected ConnectionStatusMessage(LogLevel logLevel) : base(logLevel, $"Connection to")
        {
            
        }

        public ConnectionStatusMessage(LogLevel logLevel, string message, string titleMessage = null) : base(logLevel, message, titleMessage)
        {
        }
    }

    public class CommunicationsMessage : StatusMessage
    {
        public CommDirectionEnum Direction { get; }

        public CommunicationsMessage(CommDirectionEnum direction, string data) : base(LogLevel.Debug)
        {
            Direction = direction;
            Message = $"{ControlCharacters.Prettify(data)}";
        }

        public override string ToString()
        {
            return $"[{(Direction == CommDirectionEnum.Sent ? 'S' : 'R')}] >> {Message}";
        }
    }

    public class ItemReadStatusMessage : StatusMessage
    {
        public ItemReadStatusMessage(LogLevel logLevel, ICollection<ItemMetadata> itemMetadata,
            ICollection<ItemValue> itemValues)
            : base(logLevel)
        {
            TotalCount = ItemMetadata.Count;
            ReadCount = ItemValues.Count;
            ItemMetadata = itemMetadata;
            ItemValues = itemValues;
        }

        public ICollection<ItemMetadata> ItemMetadata { get; }
        public ICollection<ItemValue> ItemValues { get; }

        public int ReadCount { get; set; }

        public int TotalCount { get; set; }

        public override string ToString()
        {
            return $"Read {ReadCount} of {TotalCount} items.";
        }
    }
}