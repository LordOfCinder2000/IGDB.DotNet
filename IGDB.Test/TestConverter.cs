using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IGDB.Tests;

public class TestConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType
        && typeToConvert.GetGenericTypeDefinition() == typeof(PagedResultDto<>);

    public override JsonConverter CreateConverter(
        Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(typeToConvert.IsGenericType &&
            typeToConvert.GetGenericTypeDefinition() == typeof(IdentityOrValue<>));

        Type elementType = typeToConvert.GetGenericArguments()[0];

        return (JsonConverter)Activator.CreateInstance(
            typeof(TestConverterOfT<>)
                .MakeGenericType(new Type[] { elementType }),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: null,
            culture: null)!;
    }
}

public class TestConverterOfT<T> : JsonConverter<PagedResultDto<T>>
{
    public override PagedResultDto<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, PagedResultDto<T> value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}