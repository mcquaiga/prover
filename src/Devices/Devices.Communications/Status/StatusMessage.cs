using System.Collections.Generic;
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
        public static StatusMessage Debug(string message) => new StatusMessage(LogLevel.Debug, message);
        public static StatusMessage Warning(string message) => new StatusMessage(LogLevel.Warning, message);
        public static StatusMessage Error(string message) => new StatusMessage(LogLevel.Error, message);
        public static StatusMessage Info(string message) => new StatusMessage(LogLevel.Information, message);

        public static CommunicationsMessage DataRecieved(string data) => new CommunicationsMessage(LogLevel.Debug, CommDirectionEnum.Received, data);
        public static CommunicationsMessage DataSent(string data) => new CommunicationsMessage(LogLevel.Debug, CommDirectionEnum.Sent, data);
    }

    public class StatusMessage
    {
        protected StatusMessage(LogLevel logLevel)
        {
            LogLevel = logLevel;
            Message = string.Empty;
        }

        public StatusMessage(LogLevel logLevel, string message)
        {
            LogLevel = logLevel;
            Message = message;
        }

        public LogLevel LogLevel { get; protected set; }
        protected string Message { get; set; }

        public override string ToString()
        {
            return $"{Message}";
        }
    }

    public class CommunicationsMessage : StatusMessage
    {
        public CommDirectionEnum Direction { get; }

        public CommunicationsMessage(LogLevel logLevel, CommDirectionEnum direction, string data) : base(logLevel)
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