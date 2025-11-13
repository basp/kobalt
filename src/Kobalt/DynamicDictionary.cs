using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;

namespace Kobalt;

public class DynamicDictionary : DynamicObject, IDictionary<string, object?>
{
    protected readonly IDictionary<string, object?> items;
    protected readonly StringComparer comparer;
    
    public DynamicDictionary()
        : this(new Dictionary<string, object?>())
    {
    }

    public DynamicDictionary(StringComparer comparer)
        : this(new Dictionary<string, object?>(), comparer)
    {
    }
    
    public DynamicDictionary(IDictionary<string, object?> items)
        : this(items, StringComparer.InvariantCulture)
    {
    }
    
    public DynamicDictionary(
        IDictionary<string, object?> items,
        StringComparer comparer)
    {
        this.items = new Dictionary<string, object?>(items, comparer);
        this.comparer = comparer;
    }
    
    public object? this[string key]
    {
        get
        {
            if (this.items.TryGetValue(key, out var value))
            {
                return value;
            }

            return this.MissingKeyBehavior.Handle(key, this);
        }
        set => this.items[key] = 
            this.ValueConverter.Convert(value);
    }

    public int Count => this.items.Count;

    public bool IsReadOnly => this.items.IsReadOnly;

    public ICollection<string> Keys => this.items.Keys;

    public ICollection<object?> Values => this.items.Values;

    public IMissingKeyBehavior MissingKeyBehavior { get; set; } =
        Kobalt.MissingKeyBehavior.Ignore;

    public IDynamicValueConverter ValueConverter { get; init; } =
        DynamicValueConverter.None;
    
    public override bool TryGetMember(
        GetMemberBinder binder, 
        out object? result)
    {
        if (this.TryGetValue(binder.Name, out result))
        {
            return true;
        }

        this.MissingKeyBehavior.Handle(binder.Name, this);
        return true;
    }
    
    public override bool TrySetMember(
        SetMemberBinder binder, 
        object? value)
    {
        this.items[binder.Name] = this.ValueConverter.Convert(value);
        return true;
    }
    
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() =>
        this.items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        new Dictionary<string, object?>(this.items).GetEnumerator();

    public void Add(KeyValuePair<string, object?> item)
    {
        this.items.Add(item);
    }

    public void Clear()
    {
        this.items.Clear();
    }

    public bool Contains(KeyValuePair<string, object?> item) =>
        this.items.Contains(item);

    public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
    {
        this.items.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<string, object?> item) =>
        this.items.Remove(item);

    public void Add(string key, object? value)
    {
        this.items.Add(key, value);
    }

    public bool ContainsKey(string key) =>
        this.items.ContainsKey(key);

    public bool Remove(string key) =>
        this.items.Remove(key);

    public bool TryGetValue(
        string key, 
        [MaybeNullWhen(false)] out object value) =>
            this.items.TryGetValue(key, out value);
}