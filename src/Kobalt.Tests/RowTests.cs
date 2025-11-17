using System.Text.Json;

namespace Kobalt.Tests;

public class RowTests
{
    [Fact]
    public void CreateEmptyRow()
    {
        // ReSharper disable once CollectionNeverUpdated.Local
        var row = new Row();
        
        Assert.Empty(row);
    }

    [Fact]
    public void CreateRowWithValues()
    {
        var values = new Dictionary<string, object?>
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 3,
        };
        
        var row = new Row(values);
        
        Assert.Equal(3, row.Count);
        Assert.Equal(1, row["a"]);
        Assert.Equal(2, row["b"]);
        Assert.Equal(3, row["c"]);
    }
    
    [Fact]
    public void SameContentRowsAreEqual()
    {
        var row1 = new Row
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 3,
        };

        var row2 = new Row
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 3,
        };
        
        Assert.Equal(row1, row2);
        Assert.True(row1.Equals(row2));
    }

    [Fact]
    public void DifferentRowsNotEqual()
    {
        var row1 = new Row
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 3,
        };
        
        var row2 = new Row
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 4,
        };
        
        Assert.NotEqual(row1, row2);
        Assert.False(row1.Equals(row2));
    }

    [Fact]
    public void CloneRowCreatesNewRow()
    {
        var a = new Row
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 3,
        };
        
        var b = a.Clone();
        b["a"] = 4;
        
        Assert.False(ReferenceEquals(a, b));
        Assert.Equal(1, a["a"]);
        Assert.Equal(4, b["a"]);
    }

    [Fact]
    public void CreateRowFromObject()
    {
        var obj = new
        {
            Id = 1,
            Name = "foo",
            Value = 3.14,
        };

        var row = Row.FromObject(obj);
        
        Assert.Equal(1, row["Id"]);
        Assert.Equal("foo", row["Name"]);
        Assert.Equal(3.14, row["Value"]);
    }

    [Fact]
    public void ConvertRowToObjectWithStaticType()
    {
        var row = new Row
        {
            ["Id"] = 1,
            ["Name"] = "foo",
            ["Value"] = 3.14,
        };
        
        var obj = row.ToObject<Foo>();
        
        Assert.Equal(1, obj.Id);
        Assert.Equal("foo", obj.Name);
        Assert.Equal(3.14, obj.Value);
    }

    [Fact]
    public void ConvertRowToObjectWithRuntimeType()
    {
        var row = new Row
        {
            ["Id"] = 1,
            ["Name"] = "foo",
            ["Value"] = 3.14,
        };

        var obj = row.ToObject(typeof(Foo));
        var foo = obj as Foo;
        
        Assert.NotNull(foo);
        Assert.Equal(1, foo.Id);
        Assert.Equal("foo", foo.Name);
        Assert.Equal(3.14, foo.Value);
    }

    [Fact]
    public void ConvertRowToJsonWithDefaultOptions()
    {
        var foo = new Foo
        {
            Id = 123,
            Name = "foo",
            Value = 3.14,
        };

        var row = Row.FromObject(foo);
        var json = row.ToJson();
        
        const string expected = """{"Id":123,"Name":"foo","Value":3.14}""";
        Assert.Equal(expected, json);
    }

    [Fact]
    public void ConvertRowToFormattedJson()
    {
        var foo = new Foo
        {
            Id = 123,
            Name = "foo",
            Value = 3.14,
        };
        
        var row = Row.FromObject(foo);
        var json = row.ToJson(new JsonSerializerOptions
        {
            WriteIndented = true,
            IndentSize = 4,
        });

        const string expected = """
                                {
                                    "Id": 123,
                                    "Name": "foo",
                                    "Value": 3.14
                                }
                                """;
        
        Assert.Equal(expected, json);
    }

    [Fact]
    public void CreateRowKeyForAllColumns()
    {
        var row1 = new Row
        {
            ["Id"] = 1,
            ["Name"] = "foo",
        };

        var row2 = new Row
        {
            ["Id"] = 1,
            ["Name"] = "foo",
        };

        var row3 = new Row
        {
            ["Id"] = 1,
            ["Name"] = "bar",
        };
        
        var key1 = row1.CreateKey();
        var key2 = row2.CreateKey();
        var key3 = row3.CreateKey();
        
        Assert.Equal(key1, key2);
        Assert.NotEqual(key1, key3);
    }
    
    [Fact]
    public void CreateRowKeyForSpecificColumns()
    {
        var row1 = new Row
        {
            ["Id"] = 1,
            ["Name"] = "foo",
            ["Value"] = 3.14,
        };

        var row2 = new Row
        {
            ["Id"] = 1,
            ["Name"] = "foo",
            ["Value"] = 6.28,
        };

        var row3 = new Row
        {
            ["Id"] = 1,
            ["Name"] = "bar",
            ["Value"] = 3.14,
        };
        
        var keyColumns = new[] { "Id", "Name" };
        var key1 = row1.CreateKey(keyColumns);
        var key2 = row2.CreateKey(keyColumns);
        var key3 = row3.CreateKey(keyColumns);
        
        Assert.Equal(key1, key2);
        Assert.NotEqual(key1, key3);
    }

    [Fact]
    public void RowAsSimpleObjectMapper()
    {
        var a1 = new RecordA
        {
            Id = 1,
            Name = "foo",
        };
        
        var a2 = new RecordA
        {
            Id = 2,
            Name = "bar",
        };

        // Create row from existing object.
        var r1 = Row.FromObject(a1);
        var r2 = Row.FromObject(a2);

        // Append row with additional values.
        r1["Value"] = 0.15;
        r2["Value"] = 0.33;
        
        // Map row to new object.
        var b1 = r1.ToObject<RecordB>();
        var b2 = r2.ToObject<RecordB>();
        
        Assert.Equal(a1.Id, b1.Id);
        Assert.Equal(a2.Id, b2.Id);
        
        Assert.Equal(a1.Name, b1.Name);
        Assert.Equal(a2.Name, b2.Name);
        
        Assert.Equal(0.15, b1.Value);
        Assert.Equal(0.33, b2.Value);
    }

    private record RecordA
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }

    private record RecordB
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public double Value { get; init; }
    }
    
    private class Foo
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public double Value { get; init; }
    }
}