namespace Kobalt.Tests;

public class KeyTests
{
    [Fact]
    public void SameValuesSameHash()
    {
        var k1 = new Key(1, "foo", 3.14);
        var k2 = new Key(1, "foo", 3.14);
        
        Assert.Equal(k1.GetHashCode(), k2.GetHashCode());
    }

    [Fact]
    public void SameValuesEqualKeys()
    {
        var k1 = new Key(1, "foo", 3.14);
        var k2 = new Key(1, "foo", 3.14);

        Assert.Equal(k1, k2);
    }
    
    [Fact]
    public void DifferentValuesNotEqualKeys()
    {
        var k1 = new Key(1, "foo", 3.14);
        var k2 = new Key(1, "bar", 3.14);
        
        Assert.NotEqual(k1, k2);
    }
    
    [Fact]
    public void DifferentValuesDifferentHash()
    {
        var k1 = new Key(1, "foo", 3.14);
        var k2 = new Key(1, "bar", 3.14);
        
        Assert.NotEqual(k1.GetHashCode(), k2.GetHashCode());
    }
}