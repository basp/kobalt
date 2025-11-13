namespace Kobalt;

/// <summary>
/// Provides a way to convert values before they are stored in a
/// <see cref="DynamicDictionary"/>.
/// </summary>
public interface IDynamicValueConverter
{
    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The converted value.</returns>
    object? Convert(object? value);
}