using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Kobalt;

/// <summary>
/// Represents a row of data.
/// </summary>
public class Row : DynamicDictionary, IEquatable<Row>
{
    // The caches help to avoid reflection in the GetProperties and GetFields
    // methods.
    private static readonly Dictionary<Type, List<PropertyInfo>> propertyCache = new();
    private static readonly Dictionary<Type, List<FieldInfo>> fieldCache = new();
    
    /// <summary>
    /// Creates a new empty row.
    /// </summary>
    public Row()
    {
    }

    /// <summary>
    /// Creates a new row from an existing dictionary.
    /// </summary>
    /// <param name="items">The source dictionary.</param>
    public Row(IDictionary<string, object?> items)
        : base(items)
    {
    }

    /// <summary>
    /// Creates a new row with a specific comparer.
    /// </summary>
    /// <param name="comparer">The comparer to use for this row.</param>
    public Row(StringComparer comparer)
        : base(comparer)
    {
    }

    /// <summary>
    /// Creates a new row from an existing dictionary and a specific comparer.
    /// </summary>
    /// <param name="items">The source dictionary.</param>
    /// <param name="comparer">The comparer to use for this row.</param>
    public Row(IDictionary<string, object?> items, StringComparer comparer)
        : base(items, comparer)
    {
    }

    /// <summary>
    /// A list of all columns in the row.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public IEnumerable<string> Columns
    {
        get
        {
            var keys = new List<string>(this.items.Keys);
            foreach (var key in keys)
            {
                yield return key;
            }
        }
    }
    
    /// <summary>
    /// Creates a new row from an object.
    /// </summary>
    /// <param name="obj">The source object for the row.</param>
    /// <returns>A new <see cref="Row"/> object.</returns>
    public static Row FromObject(object? obj)
    {
        var row = new Row();

        if (obj is null)
        {
            return row;
        }

        var properties = GetProperties(obj);
        var fields = GetFields(obj);

        foreach (var property in properties)
        {
            row[property.Name] = property.GetValue(obj);
        }

        foreach (var field in fields)
        {
            row[field.Name] = field.GetValue(obj);
        }

        return row;
    }
    
    /// <summary>
    /// Returns a clone of the row.
    /// </summary>
    /// <returns>A new row with the same items.</returns>
    public Row Clone() => new(this.items);

    /// <summary>
    /// Returns a <see cref="Key"/> object for all columns in the row.
    /// </summary>
    /// <returns>A <see cref="Key"/> object.</returns>
    public Key CreateKey() => this.CreateKey(this.Columns.ToArray());

    /// <summary>
    /// Creates a <see cref="Key"/> object for the specified columns.
    /// </summary>
    /// <param name="columns">The columns defining the key.</param>
    /// <returns>A <see cref="Key"/> object.</returns>
    public Key CreateKey(params string[] columns)
    {
        var arr = new object?[columns.Length];
        for (var i = 0; i < columns.Length; i++)
        {
            var column = columns[i];
            arr[i] = this.items[column];
        }

        return new Key(arr);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        
        if (obj is not Row other)
        {
            return false;
        }
        
        return this.Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode() => this.CreateKey().GetHashCode();
    
    /// <summary>
    /// Determines whether the specified row is equal to the current row.
    /// </summary>
    /// <param name="other">The other <see cref="Row"/> object.</param>
    /// <returns>
    /// <c>true</c> if the specified object is equal to the current object,
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(Row? other)
    {
        if (other is null)
        {
            return false;
        }

        if (!this.comparer.Equals(other.comparer))
        {
            return false;
        }

        if (!this.Columns.SequenceEqual(other.Columns))
        {
            return false;
        }

        foreach (var key in this.items.Keys)
        {
            var thisItem = this.items[key];
            var otherItem = other.items[key];

            if (thisItem is null || otherItem is null)
            {
                return thisItem == null && otherItem == null;
            }
            
            var compare = CreateComparer(
                thisItem.GetType(), 
                otherItem.GetType());

            if (!compare(thisItem, otherItem))
            {
                return false;
            }
        }

        return true;
    }
    
    /// <summary>
    /// Converts the row into an object.
    /// </summary>
    /// <param name="type">The type of the object to create.</param>
    /// <returns>A new instance of the given type.</returns>
    public object ToObject(Type type)
    {
        var instance = Activator.CreateInstance(type)!;

        foreach (var info in GetProperties(instance))
        {
            if (this.items.TryGetValue(info.Name, out var value) && info.CanWrite)
            {
                info.SetValue(instance, value);
            }
        }

        foreach (var info in GetFields(instance))
        {
            if (this.items.TryGetValue(info.Name, out var value))
            {
                info.SetValue(instance, value);
            }
        }

        return instance;
    }

    /// <summary>
    /// Convert and cast the row into an object.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <returns>A new object of type <c>T</c>.</returns>
    public T ToObject<T>() => (T)this.ToObject(typeof(T));
    
    /// <summary>
    /// Convert the row into a JSON string.
    /// </summary>
    /// <param name="options">The serialization options.</param>
    /// <returns>A JSON string representing the row.</returns>
    public string ToJson(JsonSerializerOptions? options = null) =>
        JsonSerializer.Serialize(this, options);

    // Experimental
    public JsonNode? ToJsonNode() => JsonNode.Parse(this.ToJson());
    
    private static List<PropertyInfo> GetProperties(object obj)
    {
        var type = obj.GetType();
        if (Row.propertyCache.TryGetValue(type, out var properties))
        {
            return properties;
        }

        const BindingFlags bindingFlags = BindingFlags.Public |
                                          BindingFlags.Instance |
                                          BindingFlags.NonPublic;
        
        properties = type
            .GetProperties(bindingFlags)
            .Where(x => x.CanRead)
            .Where(x => x.GetIndexParameters().Length == 0)
            .ToList();

        propertyCache[type] = properties;
        return properties;
    }

    private static List<FieldInfo> GetFields(object obj)
    {
        var type = obj.GetType();
        if (fieldCache.TryGetValue(type, out var fields))
        {
            return fields;
        }

        const BindingFlags bindingFlags = BindingFlags.Public |
                                          BindingFlags.Instance |
                                          BindingFlags.NonPublic;
        
        fields = [];
        foreach (var field in type.GetFields(bindingFlags))
        {
            // Skip compiler-generated fields.
            if (Attribute.IsDefined(field, typeof(CompilerGeneratedAttribute)))
            {
                continue;
            }

            fields.Add(field);
        }

        fieldCache[type] = fields;
        return fields;
    }
    
    private static Func<object, object, bool> CreateComparer(
        Type firstType,
        Type secondType)
    {
        if (firstType == secondType)
        {
            return Equals;
        }

        var firstParam =
            Expression.Parameter(typeof(object), "first");
        var secondParam =
            Expression.Parameter(typeof(object), "second");
        
        var expression =
            Expression.Equal(
                Expression.Convert(firstParam, firstType),
                Expression.Convert(secondParam, secondType));
        
        return Expression
            .Lambda<Func<object, object, bool>>(
                expression,
                firstParam,
                secondParam)
            .Compile();
    }
}