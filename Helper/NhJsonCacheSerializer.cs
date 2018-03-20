using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHibernate.Cache;
using NHibernate.Cache.Entry;
using NHibernate.Caches.Redis;
using StackExchange.Redis;
using System.Drawing;

namespace Exf.Persistance.NHibernate
{
    public class NhJsonCacheSerializer : ICacheSerializer
    {
        // By default, JSON.NET will always use Int64/Double when deserializing numbers
        // since there isn't an easy way to detect the proper number size. However,
        // because NHibernate does casting to the correct number type, it will fail.
        // Adding the type to the serialize object is what the "TypeNameHandling.All"
        // option does except that it doesn't include numbers.
        private class KeepNumberTypesConverter : JsonConverter
        {
            // We shouldn't have to account for Nullable<T> because the serializer
            // should see them as null.
            private static readonly ISet<Type> numberTypes = new HashSet<Type>(new[]
            {
                typeof(Byte), typeof(SByte),
                typeof(UInt16), typeof(UInt32), typeof(UInt64),
                typeof(Int16), typeof(Int32), typeof(Int64),
                typeof(Single), typeof(Double), typeof(Decimal),typeof(Guid)
            });

            public override bool CanConvert(Type objectType)
            {
                return numberTypes.Contains(objectType);
            }

            // JSON.NET will deserialize a value with the explicit type when 
            // the JSON object exists with $type/$value properties. So, we 
            // don't need to implement reading.
            public override bool CanRead { get { return false; } }

            public override bool CanWrite { get { return true; } }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                // CanRead is false.
                throw new NotImplementedException();
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("$type");
                var typeName = value.GetType().FullName;
                writer.WriteValue(typeName);
                writer.WritePropertyName("$value");
                writer.WriteValue(value);
                writer.WriteEndObject();
            }
        }

        private class CustomContractResolver : DefaultContractResolver
        {
            private static readonly ISet<Type> nhibernateCacheObjectTypes = new HashSet<Type>(new[]
            {
                typeof(CachedItem),
                typeof(CacheLock),
                typeof(CacheEntry),
                typeof(CollectionCacheEntry)
            });

            protected override JsonObjectContract CreateObjectContract(Type objectType)
            {
                var result = base.CreateObjectContract(objectType);

                // JSON.NET uses the default constructor (or uninitialized objects)
                // by default. Since Point is immutable, we must use the parameterized
                // constructor. Since we don't need to take a dependency on JSON.NET
                // in the model, we can't use [JsonConstructor]. It is explicitly 
                // emulated here.
                if (objectType == typeof(Point))
                {
                    result.CreatorParameters.Add(new JsonProperty()
                    {
                        PropertyName = "x",
                        PropertyType = typeof(Int32)
                    });
                    result.CreatorParameters.Add(new JsonProperty()
                    {
                        PropertyName = "y",
                        PropertyType = typeof(Int32)
                    });
                    result.OverrideCreator = (args) =>
                    {
                        return new Point((Int32)args[0], (Int32)args[1]);
                    };
                }
                // By default JSON.NET will only use the public constructors that 
                // require parameters such as ISessionImplementor. Because the 
                // NHibernate cache objects use internal constructors that don't 
                // do anything except initialize the fields, it's much easier 
                // (no constructor lookup) to just get an uninitialized object and 
                // fill in the fields.
                else if (nhibernateCacheObjectTypes.Contains(objectType))
                {
                    result.DefaultCreator = () => FormatterServices.GetUninitializedObject(objectType);
                }

                return result;
            }

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                if (nhibernateCacheObjectTypes.Contains(type))
                {
                    // By default JSON.NET will serialize the NHibernate objects with
                    // their public properties. However, the backing fields/property
                    // names don't always match up. Therefore, we *only* use the fields
                    // so that we can get/set the correct value.
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .Select(f => base.CreateProperty(f, memberSerialization));

                    var result = fields.Select(p =>
                    {
                        p.Writable = true;
                        p.Readable = true;
                        return p;
                    }).ToList();
                    return result;
                }
                else
                {
                    return base.CreateProperties(type, memberSerialization);
                }
            }
        }

        private readonly JsonSerializerSettings settings;

        public NhJsonCacheSerializer()
        {
            this.settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;
            settings.Converters.Add(new KeepNumberTypesConverter());
            settings.ContractResolver = new CustomContractResolver();
        }

        public RedisValue Serialize(object value)
        {
            if (value == null) return RedisValue.Null;

            var result = JsonConvert.SerializeObject(value, Formatting.None, settings);
            return result;
        }

        public object Deserialize(RedisValue value)
        {
            if (value.IsNull) return null;

            var result = JsonConvert.DeserializeObject(value, settings);
            return result;
        }
    }
}