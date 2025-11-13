namespace Kobalt.Tests;

public class DynamicDictionaryTests
{
    [Fact]
    public void CaseSensitiveByDefault()
    {
        var d = new DynamicDictionary
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 3,
            MissingKeyBehavior = MissingKeyBehavior.Ignore,
        };

        Assert.Equal(1, d["a"]);
        Assert.Equal(2, d["b"]);
        Assert.Equal(3, d["c"]);
        Assert.Null(d["A"]);
        Assert.Null(d["d"]);
    }

    [Fact]
    public void UseCaseInsensitiveComparer()
    {
        var d = new DynamicDictionary(StringComparer.InvariantCultureIgnoreCase)
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 3,
        };

        Assert.Equal(1, d["a"]);
        Assert.Equal(1, d["A"]);
        Assert.Equal(2, d["b"]);
        Assert.Equal(2, d["B"]);
        Assert.Equal(3, d["c"]);
        Assert.Equal(3, d["C"]);
        Assert.Null(d["d"]);
    }

    [Fact]
    public void DynamicFromExisting()
    {
        var existing = new Dictionary<string, object?>
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 3,
        };

        dynamic d = new DynamicDictionary(existing)
        {
            MissingKeyBehavior = MissingKeyBehavior.Throw,
        };

        Assert.Equal(1, d.a);
        Assert.Equal(2, d.b);
        Assert.Equal(3, d.c);
        Assert.Throws<KeyNotFoundException>(() => d.A);
        Assert.Throws<KeyNotFoundException>(() => d.d);
    }

    [Fact]
    public void DynamicFromExistingWithComparer()
    {
        var existing = new Dictionary<string, object?>
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 3,
        };

        dynamic d = new DynamicDictionary(
            existing,
            StringComparer.InvariantCultureIgnoreCase)
        {
            MissingKeyBehavior = MissingKeyBehavior.Throw,
        };

        Assert.Equal(1, d.A);
        Assert.Equal(1, d.a);
        Assert.Equal(2, d.B);
        Assert.Equal(2, d.b);
        Assert.Equal(3, d.C);
        Assert.Equal(3, d.c);
        Assert.Throws<KeyNotFoundException>(() => d.d);
    }
    
    [Fact]
    public void ThrowOnMissingCaseSensitiveKey()
    {
        var d = new DynamicDictionary
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 3,
            MissingKeyBehavior = MissingKeyBehavior.Throw,       
        };
        
        Assert.Equal(1, d["a"]);
        Assert.Equal(2, d["b"]);
        Assert.Equal(3, d["c"]);
        Assert.Throws<KeyNotFoundException>(() => d["A"]);
        Assert.Throws<KeyNotFoundException>(() => d["d"]);
    }
    
    [Fact]
    public void ThrowOnMissingCaseInsensitiveKey()
    {
        var d = new DynamicDictionary(StringComparer.InvariantCultureIgnoreCase)
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 3,
            MissingKeyBehavior = MissingKeyBehavior.Throw,      
        };
        
        Assert.Equal(1, d["a"]);
        Assert.Equal(1, d["A"]);
        Assert.Equal(2, d["b"]);
        Assert.Equal(2, d["B"]);
        Assert.Equal(3, d["c"]);
        Assert.Equal(3, d["C"]);
        Assert.Throws<KeyNotFoundException>(() => d["d"]);
    }

    [Fact]
    public void UseDynamicCaseSensitiveAccess()
    {
        dynamic d = new DynamicDictionary
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 3,
            MissingKeyBehavior = MissingKeyBehavior.Ignore,
        };

        d.A = 4;
        d.D = 5;
        
        Assert.Equal(1, d.a);
        Assert.Equal(4, d.A);
        Assert.Equal(2, d.b);
        Assert.Equal(3, d.c);
        Assert.Equal(5, d.D);
        Assert.Null(d.d);
    }
    
    [Fact]
    public void UseDynamicCaseInsensitiveAccess()
    {
        dynamic d = new DynamicDictionary(StringComparer.InvariantCultureIgnoreCase)
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 3,
        };
        
        Assert.Equal(1, d.a);
        Assert.Equal(1, d.A);
        Assert.Equal(2, d.b);
        Assert.Equal(2, d.B);
        Assert.Equal(3, d.c);
        Assert.Equal(3, d.C);
        Assert.Null(d.d);
        Assert.Null(d.D);
    }

    [Fact]
    public void NoValueConversionByDefault()
    {
        dynamic d = new DynamicDictionary();
        d.a = null;
        
        Assert.Null(d.a);
    }

    [Fact]
    public void DbNullValueConversion()
    {
        dynamic d = new DynamicDictionary()
        {
            ValueConverter = DynamicValueConverter.Database,
        };

        d.a = null;
        
        Assert.Equal(DBNull.Value, d.a);
    }
}
