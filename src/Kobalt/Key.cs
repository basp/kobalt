namespace Kobalt;

/// <summary>
/// Represents a key based on a set of objects.
/// </summary>
/// <remarks>
/// The main purposes of this class are to provide a way to easily compare
/// and hash sets of objects.
/// </remarks>
public class Key
{
    private readonly object?[] values;

    /// <summary>
    /// Constructs a composite key for the specified values.
    /// </summary>
    /// <param name="values">The values of the key.</param>
    public Key(params object?[] values)
    {
        this.values = values;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        
        if (obj is not Key other)
        {
            return false;
        }

        if (this.values.Length != other.values.Length)
        {
            return false;
        }

        return this.values.SequenceEqual(other.values);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var result = 0;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var value in this.values)
        {
            if (value == null)
            {
                continue;
            }

            result ^= value.GetHashCode();
        }

        return result;
    }
}