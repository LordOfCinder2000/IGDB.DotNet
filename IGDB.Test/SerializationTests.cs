using IGDB.Models;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Apicalypse;
using System.Linq;
using Apicalypse.Extensions;
using Apicalypse.NamingPolicies;

namespace IGDB.Tests;

public class SerializationTests
{
    [Test]
    public void IdentityConverter_Should_Serialize_and_Deserialize_Id()
    {
        var tagNumber = TagNumberHelper.Generate(TagType.Genre, 5);
        var str = new QueryBuilder<Game>(new Apicalypse.Configuration.QueryBuilderOptions { NamingPolicy = NamingPolicy.SnakeCase})
            .Select(g => new {g.Name})
            .Where(g => new int[] { tagNumber }.IsAnyIn(g.Tags)).Build();

        var game = new Game();
        game.ParentGame = new IdentityOrValue<Game>(3);
        game.ReleaseDates = new IdentitiesOrValues<ReleaseDate>(new List<long> { 1, 2, 3 });

        var jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.Converters.Add(new UnixTimestampConverter());
        jsonSerializerOptions.Converters.Add(new IdentityConverter());
        jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        jsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
        //var serialized = JsonSerializer.Serialize(game, jsonSerializerOptions);
        var serialized = "[{ \"id\": 202862, \"category\": 0, \"name\": \"Drop\" },{ \"id\": 202862, \"category\": 0, \"name\": \"Drop\" }]";
        var deserialized = JsonSerializer.Deserialize<Game[]>(serialized, jsonSerializerOptions);

        var serialized2 = JsonSerializer.Serialize(deserialized, jsonSerializerOptions);
    }

    [Test]
    public async Task TestRefit()
    {
        var jsonRaw = "[{ \"id\": 202862, \"category\": 0, \"name\": \"Drop\" },{ \"id\": 202862, \"category\": 0, \"name\": \"Drop\" }]";

        var currentThread = Thread.CurrentThread.ManagedThreadId;
        //var result = await FromHttpContentAsync<PagedResultDto<Game>>(jsonRaw);
        await Test();
        var currentThread2 = Thread.CurrentThread.ManagedThreadId;
    }

    public async Task<T?> FromHttpContentAsync<T>(string jsonRaw)
    {
        var jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.Converters.Add(new UnixTimestampConverter());
        jsonSerializerOptions.Converters.Add(new IdentityConverter());
        jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        jsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();

        var targetType = typeof(T);

        if (targetType!.GetTypeInfo().IsGenericType &&
            (targetType!.GetGenericTypeDefinition() == typeof(PagedResultDto<>) ||
            targetType!.GetGenericTypeDefinition() == typeof(IPagedResult<>)))
        {
            //items type
            var itemType = targetType.GetGenericArguments()[0];
            itemType = itemType.MakeArrayType();

            var deserializeMethod = typeof(JsonSerializer).GetMethod(nameof(JsonSerializer.Deserialize), new Type[] { typeof(string), typeof(JsonSerializerOptions) });


            deserializeMethod = deserializeMethod!.MakeGenericMethod(itemType);

            var currentThread = Thread.CurrentThread.ManagedThreadId;

            var result = (IReadOnlyList<dynamic>)deserializeMethod.Invoke(null, new object[] { jsonRaw, jsonSerializerOptions })!;

            var currentThread2 = Thread.CurrentThread.ManagedThreadId;

            if (result != null)
            {
                return (T?)Activator.CreateInstance(typeof(T), 999, result);
            }
        }

        return JsonSerializer.Deserialize<T>(jsonRaw, jsonSerializerOptions);
    }

    public async Task Test()
    {
        var game = new Game
        {
            Name = "test"
        };

        var testMethod = typeof(TestGenerics).GetMethod(nameof(TestGenerics.GenericsAsync))!.MakeGenericMethod(typeof(Game));

        var currentThread = Thread.CurrentThread.ManagedThreadId;

        var result = await (Task<dynamic>)testMethod.Invoke(null, new object[] { game })!;

        var currentThread2 = Thread.CurrentThread.ManagedThreadId;
    }
}


public class TestGenerics
{
    public static async Task<object> GenericsAsync<T>(T game)
    {
        var currentThread = Thread.CurrentThread.ManagedThreadId;

        await Task.Delay(3000);

        var currentThread2 = Thread.CurrentThread.ManagedThreadId;

        return new
        {
            HelloWorld = 1
        };
    }
}


public class PagedResultDto<T> : ListResultDto<T>, IPagedResult<T>
{
    /// <inheritdoc />
    public long TotalCount { get; set; } //TODO: Can be a long value..?

    /// <summary>
    /// Creates a new <see cref="PagedResultDto{T}"/> object.
    /// </summary>
    public PagedResultDto()
    {

    }

    /// <summary>
    /// Creates a new <see cref="PagedResultDto{T}"/> object.
    /// </summary>
    /// <param name="totalCount">Total count of Items</param>
    /// <param name="items">List of items in current page</param>
    public PagedResultDto(long totalCount, IReadOnlyList<T> items)
        : base(items)
    {
        TotalCount = totalCount;
    }
}

public class ListResultDto<T>
{
    /// <inheritdoc />
    public IReadOnlyList<T> Items
    {
        get { return _items ?? (_items = new List<T>()); }
        set { _items = value; }
    }
    private IReadOnlyList<T> _items;

    /// <summary>
    /// Creates a new <see cref="ListResultDto{T}"/> object.
    /// </summary>
    public ListResultDto()
    {

    }

    /// <summary>
    /// Creates a new <see cref="ListResultDto{T}"/> object.
    /// </summary>
    /// <param name="items">List of items</param>
    public ListResultDto(IReadOnlyList<T> items)
    {
        Items = items;
    }
}

public interface IPagedResult<T>
{
}

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => ToSnakeCase(name);

    public string ToSnakeCase(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        StringBuilder stringBuilder = new StringBuilder(str.Length + Math.Min(2, str.Length / 5));
        UnicodeCategory? unicodeCategory = null;
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            if (c == '_')
            {
                stringBuilder.Append('_');
                unicodeCategory = null;
                continue;
            }

            UnicodeCategory unicodeCategory2 = char.GetUnicodeCategory(c);
            switch (unicodeCategory2)
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                    if (unicodeCategory == UnicodeCategory.SpaceSeparator || unicodeCategory == UnicodeCategory.LowercaseLetter || (unicodeCategory != UnicodeCategory.DecimalDigitNumber && unicodeCategory.HasValue && i > 0 && i + 1 < str.Length && char.IsLower(str[i + 1])))
                    {
                        stringBuilder.Append('_');
                    }

                    c = char.ToLower(c);
                    break;
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.DecimalDigitNumber:
                    if (unicodeCategory == UnicodeCategory.SpaceSeparator)
                    {
                        stringBuilder.Append('_');
                    }

                    break;
                default:
                    if (unicodeCategory.HasValue)
                    {
                        unicodeCategory = UnicodeCategory.SpaceSeparator;
                    }

                    continue;
            }

            stringBuilder.Append(c);
            unicodeCategory = unicodeCategory2;
        }

        return stringBuilder.ToString();
    }
}

