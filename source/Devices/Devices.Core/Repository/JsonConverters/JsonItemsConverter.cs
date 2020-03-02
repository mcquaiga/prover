using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devices.Core.Items;
using Devices.Core.Items.Descriptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prover.Shared.Json;

namespace Devices.Core.Repository.JsonConverters
{
    public class ItemDescriptionSerializer : JsonSerializer
    {
        public ItemDescriptionSerializer()
        {
            
        }

        public new static ItemDescriptionSerializer Create()
        {
            var i = new ItemDescriptionSerializer();
            i.Converters.Add(new ItemDescriptionConverter());
            return i;
        }
    }

    public class ItemDescriptionConverter : JsonCreationConverter<ItemDescription>
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        protected override ItemDescription Create(Type objectType, JObject jObject)
        {
            if (jObject.TryGetValue("numericvalue", StringComparison.CurrentCultureIgnoreCase, out var numberToken))
            {
                return new ItemDescriptionWithNumericValue();
            }

            return new ItemDescription();
        }
    }

    public class JsonItemsConverter : JsonCreationConverter<ItemMetadata>
    {
        public JsonItemsConverter(IStreamReader streamReader)
        {
            _streamReader = streamReader;
        }

        private readonly IStreamReader _streamReader;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        protected override ItemMetadata Create(Type objectType, JObject jObject)
        {
            var itemToken = jObject["ItemDescriptions"];
            if (itemToken != null && itemToken.HasValues)
            {
                var itemName = itemToken.First.Value<string>("definitionPath");
                if (itemName != null)
                {
                    var task = Task.Run(async () => await GetItemDescriptionsAsync(itemName));
                    var desc = task.Result;
                    return new ItemMetadata(desc.ToList());
                }

                return new ItemMetadata(itemToken.ToObject<ICollection<ItemDescription>>(ItemDescriptionSerializer.Create()));
            }

            return new ItemMetadata();
        }

        protected async Task<IEnumerable<ItemDescription>> GetItemDescriptionsAsync(string name)
        {
            using (var sr = _streamReader.GetItemDefinitionsReader(name))
            {
                using (var reader = new JsonTextReader(sr))
                {
                    var defJObject = await JObject.LoadAsync(reader);

                    var descTypeString = defJObject["type"]?.Value<string>();

                    if (string.IsNullOrEmpty(descTypeString))
                        throw new NullReferenceException(
                            $"Could not find [type] property in {name} definition. Verify the JSON document is valid.");
                    
                    var type = Type.GetType(descTypeString);

                    if (type == null)
                        throw new TypeLoadException($"Could not load type for {descTypeString}.");

                    var items = defJObject["ItemDescriptions"]?.ToString();

                    if (items == null)
                        throw new NullReferenceException(
                            $"Could not find [ItemDescriptions] property in item type {name}. Verify the JSON document is valid.");

                    return (IEnumerable<ItemDescription>)
                        JsonConvert.DeserializeObject(items, typeof(ICollection<>).MakeGenericType(type));
                }
            }
        }
    }
}