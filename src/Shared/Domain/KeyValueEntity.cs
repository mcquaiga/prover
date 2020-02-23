using System;
using System.Text.Json;


namespace Shared.Domain
{
    public class KeyValueEntity : IKeyValueEntity
    {
        public string Key { get; set; }

        public virtual string Value { get; set; }

        public virtual T GetObject<T>() where T : class
        {
            return JsonSerializer.Deserialize<T>(Value);
        }
    }

    [Serializable]
    public class KeyValueEntity<T> : KeyValueEntity
        where T : class
    {
        public T Entity { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(Entity);
        }
    }
}