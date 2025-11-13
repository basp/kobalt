namespace Kobalt;

/// <inheritdoc/>
public class DynamicValueConverter : IDynamicValueConverter
{
    /// <summary>
    /// No conversion is performed.
    /// </summary>
    /// <remarks>
    /// This is the default behavior.
    /// </remarks>   
    public static readonly IDynamicValueConverter None =
        new DynamicValueConverter(x => x);

    /// <summary>
    /// Converts a <c>null</c> value to <see cref="DBNull.Value"/>.
    /// </summary>
    /// <remarks>
    /// This is useful when values in the dynamic dictionary are used for
    /// SQL commands.
    /// </remarks>
    public static readonly IDynamicValueConverter Database =
        new DynamicValueConverter(x => x == null ? DBNull.Value : null);
    
    private readonly Func<object?, object?> converter;
    
    private DynamicValueConverter(Func<object?, object?> converter)
    {
        this.converter = converter;
    }
    
    /// <inheritdoc/>
    public object? Convert(object? value) => this.converter(value);
}