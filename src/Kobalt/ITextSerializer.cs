namespace Kobalt;

public interface ITextSerializer
{
    T Deserialize<T>(string text);

    string Serialize<T>(T obj);
}