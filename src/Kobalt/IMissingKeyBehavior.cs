namespace Kobalt;

/// <summary>
/// Defines the behavior on how a <see cref="DynamicDictionary"/> should handle
/// reads of a missing key.
/// </summary>
public interface IMissingKeyBehavior
{
    /// <summary>
    /// Handles the missing key for the specified row.
    /// </summary>
    /// <param name="key">The name of the missing key.</param>
    /// <param name="dictionary">
    /// The dictionary which was expected to contain the key.
    /// </param>
    public object? Handle(string key, DynamicDictionary dictionary);
}