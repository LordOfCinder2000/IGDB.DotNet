using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IGDB
{
    public class IdentityConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
            => typeToConvert.IsGenericType
            && (typeToConvert.GetGenericTypeDefinition() == typeof(IdentityOrValue<>)
            || typeToConvert.GetGenericTypeDefinition() == typeof(IdentitiesOrValues<>));

        public override JsonConverter CreateConverter(
            Type typeToConvert, JsonSerializerOptions options)
        {
            Type elementType = typeToConvert.GetGenericArguments()[0];
            JsonConverter converter = default;
            if (typeToConvert.GetGenericTypeDefinition() == typeof(IdentityOrValue<>))
            {
                Debug.Assert(typeToConvert.IsGenericType &&
                typeToConvert.GetGenericTypeDefinition() == typeof(IdentityOrValue<>));

                converter = (JsonConverter)Activator.CreateInstance(
                    typeof(IdentityOrValueConverterOfT<>)
                        .MakeGenericType(new Type[] { elementType }),
                    BindingFlags.Instance | BindingFlags.Public,
                    binder: null,
                    args: null,
                    culture: null);
            }
            else if (typeToConvert.GetGenericTypeDefinition() == typeof(IdentitiesOrValues<>))
            {
                Debug.Assert(typeToConvert.IsGenericType &&
                typeToConvert.GetGenericTypeDefinition() == typeof(IdentitiesOrValues<>));

                converter = (JsonConverter)Activator.CreateInstance(
                    typeof(IdentitiesOrValuesConverterOfT<>)
                        .MakeGenericType(new Type[] { elementType }),
                    BindingFlags.Instance | BindingFlags.Public,
                    binder: null,
                    args: null,
                    culture: null);
            }

            return converter;
        }
    }

    public class IdentityOrValueConverterOfT<T> : JsonConverter<IdentityOrValue<T>> where T : class
    {
        public override IdentityOrValue<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // object
                return (IdentityOrValue<T>)Activator.CreateInstance(typeToConvert, JsonSerializer.Deserialize<T>(ref reader));
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                // int ids
                return (IdentityOrValue<T>)Activator.CreateInstance(typeof(IdentityOrValue<T>), reader.GetInt64());
            }

            throw new InvalidCastException("Could not Deserialize JSON into identity");
        }

        public override void Write(Utf8JsonWriter writer, IdentityOrValue<T> value, JsonSerializerOptions options)
        {
            if (value.Value is null)
            {
                JsonSerializer.Serialize(writer, value.Id, options);
            }
            else if (value.Id is null)
            {
                JsonSerializer.Serialize(writer, value.Value, options);
            }
        }
    }

    public class IdentitiesOrValuesConverterOfT<T> : JsonConverter<IdentitiesOrValues<T>> where T : class
    {
        public override IdentitiesOrValues<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new InvalidCastException("Cannot convert non-array JSON value to IdentitiesOrValues type");
            }

            // Read first value in array
            var values = new List<object>();
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    var obj = JsonSerializer.Deserialize<T>(ref reader, options);
                    // objects
                    values.Add(obj);
                }
                else if (reader.TokenType == JsonTokenType.Number)
                {
                    // int ids
                    values.Add(reader.GetInt64());
                }
            }

            // If any are objects, it means the IDs should be ignored
            if (values.All(v => v.GetType().IsAssignableFrom(typeof(long))))
            {
                return (IdentitiesOrValues<T>)Activator.CreateInstance(typeof(IdentitiesOrValues<T>), values.Cast<long>().ToList());
            }

            var objects = values.Where(v => !v.GetType().IsAssignableFrom(typeof(long))).Cast<T>().ToList();
            var ctor = typeToConvert.GetConstructor(new[] { typeof(List<T>) });
            return (IdentitiesOrValues<T>)ctor.Invoke(new[] { objects });
        }

        public override void Write(Utf8JsonWriter writer, IdentitiesOrValues<T> value, JsonSerializerOptions options)
        {
            if (value.Values is null)
            {
                JsonSerializer.Serialize(writer, value.Ids, options);
            }
            else if (value.Ids is null)
            {
                JsonSerializer.Serialize(writer, value.Values, options);
            }
        }
    }
}