using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Mscribel.FixedWidth;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Kobalt.Belastingdienst;

public class BankrekeningenParser
{
    private const string IsVoorloopPattern = "^.{12}00";
    private const string IsAlgemeenRecordPattern = "^.{12}10";
    private const string IsWaarderecord1Pattern = "^.{12}20.{9}(GEEN GEGEVENS|ONDER NORM)";
    private const string IsWaarderecord2Pattern = "^.{12}20.{9}REKENINGNR/BANKCODE";
    private const string IsWaarderecord3Pattern = "^.{12}20.{9}NAAM REKENINGHOUDER";
    private const string IsWaarderecord4Pattern = "^.{12}20.{9}RENTE- EN SALDOBEDRAG";
    private const string IsSluitrecordPattern = "^.{12}99";
    
    // ReSharper disable once MemberCanBePrivate.Global
    protected readonly ILogger logger;

    protected BankrekeningenParser(ILogger logger)
    {
        this.logger = logger;
    }

#pragma warning disable SYSLIB1045
    private static readonly Regex isVoorlooprecordRegex = 
        new(BankrekeningenParser.IsVoorloopPattern, RegexOptions.Compiled);
    private static readonly Regex isAlgemeenRecordRegex = 
        new(BankrekeningenParser.IsAlgemeenRecordPattern, RegexOptions.Compiled);
    private static readonly Regex isWaarderecord1Regex = 
        new(BankrekeningenParser.IsWaarderecord1Pattern, RegexOptions.Compiled);
    private static readonly Regex isWaarderecord2Regex = 
        new(BankrekeningenParser.IsWaarderecord2Pattern, RegexOptions.Compiled);
    private static readonly Regex isWaarderecord3Regex = 
        new(BankrekeningenParser.IsWaarderecord3Pattern, RegexOptions.Compiled);
    private static readonly Regex isWaarderecord4Regex = 
        new(BankrekeningenParser.IsWaarderecord4Pattern, RegexOptions.Compiled);
    private static readonly Regex isSluitrecordRegex = 
        new(BankrekeningenParser.IsSluitrecordPattern, RegexOptions.Compiled);
#pragma warning restore SYSLIB1045
    
    private static readonly TextSerializer<Voorlooprecord> voorlooprecordSerializer =
        new();

    private static readonly TextSerializer<AlgemeenRecord> algemeenRecordSerializer =
        new();

    private static readonly TextSerializer<Waarderecord1> waardeRecord1Serializer =
        new();

    private static readonly TextSerializer<Waarderecord2> waardeRecord2Serializer =
        new();

    private static readonly TextSerializer<Waarderecord3> waardeRecord3Serializer =
        new();

    private static readonly TextSerializer<Waarderecord4> waardeRecord4Serializer =
        new();

    private static readonly TextSerializer<Sluitrecord> sluitrecordSerializer =
        new();
    
    protected virtual void OnVoorlooprecord(Voorlooprecord record)
    {
    }

    protected virtual void OnAlgemeenRecord(AlgemeenRecord record)
    {
    }

    protected virtual void OnWaarderecord1(Waarderecord1 record)
    {
    }

    protected virtual void OnWaarderecord2(Waarderecord2 record)
    {
    }

    protected virtual void OnWaarderecord3(Waarderecord3 record)
    {
    }

    protected virtual void OnWaarderecord4(Waarderecord4 record)
    {
    }

    protected virtual void OnSluitrecord(Sluitrecord record)
    {
    }

    public void Parse(Stream stream)
    {
        var parsers = this.GetParsers();
        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var parser in parsers)
            {
                // ReSharper disable once InvertIf
                if (parser.Key.IsMatch(line))
                {
                    parser.Value(line);
                    break;
                }
            }
        }
    }

    private Dictionary<Regex, Action<string>> GetParsers() => new()
    {
        [BankrekeningenParser.isVoorlooprecordRegex] = line =>
        {
            var record =
                BankrekeningenParser
                    .voorlooprecordSerializer
                    .Deserialize(line);
            this.logger.LogTrace(
                "OnVoorlooprecord: {IndicatiefInzender}", 
                record.IndicatiefInzender);
            this.OnVoorlooprecord(record);
        },
        [BankrekeningenParser.isAlgemeenRecordRegex] = line =>
        {
            var record =
                BankrekeningenParser
                    .algemeenRecordSerializer
                    .Deserialize(line);
            this.logger.LogTrace("OnAlgemeenRecord: {BSN}", record.BSN);
            this.OnAlgemeenRecord(record);
        },
        [BankrekeningenParser.isWaarderecord1Regex] = line =>
        {
            var record =
                BankrekeningenParser
                    .waardeRecord1Serializer
                    .Deserialize(line);
            this.logger.LogTrace("OnWaarderecord1: {waarde}", record.Waarde);
            this.OnWaarderecord1(record);
        },
        [BankrekeningenParser.isWaarderecord2Regex] = line =>
        {
            var record =
                BankrekeningenParser
                    .waardeRecord2Serializer
                    .Deserialize(line);
            this.logger.LogTrace("OnWaarderecord2: {waarde}", record.Waarde);
            this.OnWaarderecord2(record);
        },
        [BankrekeningenParser.isWaarderecord3Regex] = line =>
        {
            var record =
                BankrekeningenParser
                    .waardeRecord3Serializer
                    .Deserialize(line);
            this.logger.LogTrace("OnWaarderecord3: {waarde}", record.Waarde);
            this.OnWaarderecord3(record);
        },
        [BankrekeningenParser.isWaarderecord4Regex] = line =>
        {
            var record =
                BankrekeningenParser
                    .waardeRecord4Serializer
                    .Deserialize(line);
            this.logger.LogTrace("OnWaardeRecord4: {waarde}", record.Waarde);
            this.OnWaarderecord4(record);
        },
        [BankrekeningenParser.isSluitrecordRegex] = line =>
        {
            var record =
                BankrekeningenParser
                    .sluitrecordSerializer
                    .Deserialize(line);
            this.logger.LogTrace(
                "OnSluitrecord: {AantalRecords}", 
                record.AantalRecords);
            this.OnSluitrecord(record);
        },
    };

    [TextSerializable]
    public class Voorlooprecord
    {
        [TextField(1, 12)] public string IndicatiefInzender { get; set; } = string.Empty;

        [TextField(13, 2)] public int Recordcode { get; set; }

        [TextField(15, 10)] public string Afnametype { get; set; } = string.Empty;

        [TextField(25, 4)] public int Belastingjaar { get; set; }

        [TextField(29, 22)] public string Naam { get; set; } = string.Empty;

        [TextField(51, 20)] public string Adres { get; set; } = string.Empty;

        [TextField(71, 20)] public string PostcodeEnWoonplaats { get; set; } = string.Empty;

        [TextField(91, 2)] public int Mediumcode { get; set; }

        [TextField(93, 1)] public char Density { get; set; }

        [TextField(94, 1)] public char Characterset { get; set; }

        [TextField(95, 1)] public char Labelcode { get; set; }
    }

    [TextSerializable]
    public class AlgemeenRecord
    {
        [TextField(1, 12)] public string IndicatiefInzender { get; set; } = string.Empty;

        [TextField(13, 2)] public int Recordcode { get; set; }

        [TextField(15, 9)] public string BSN { get; set; } = string.Empty;

        [TextField(24, 8)] public string Geboortedatum { get; set; } = string.Empty;

        [TextField(32, 49)] public string Naam { get; set; } = string.Empty;

        [TextField(81, 5)] public string Voorletters { get; set; } = string.Empty;

        [TextField(86, 8)] public string Voorvoegsels { get; set; } = string.Empty;

        [TextField(94, 25)] public string Straatnaam { get; set; } = string.Empty;

        [TextField(119, 5)] public string Huisnummer { get; set; } = string.Empty;

        [TextField(124, 4)] public string Toevoeging { get; set; } = string.Empty;

        [TextField(128, 6)] public string Postcode { get; set; } = string.Empty;

        [TextField(134, 24)] public string Plaatsnaam { get; set; } = string.Empty;
    }

    [TextSerializable]
    public class Waarderecord1
    {
        [TextField(1, 12)] public string IndicatiefInzender { get; set; } = string.Empty;

        [TextField(13, 2)] public int Recordcode { get; set; }

        [TextField(15, 9)] public string BSN { get; set; } = string.Empty;

        [TextField(24, 25)] public string Rubriek { get; set; } = string.Empty;

        [TextField(49, 49)] public string Waarde { get; set; } = string.Empty;

        [TextField(98, 2)] public int Controlecode { get; set; }

        [TextField(100, 54)] public string Reserve { get; set; } = string.Empty;
    }

    [TextSerializable]
    public class Waarderecord2
    {
        [TextField(1, 12)] public string IndicatiefInzender { get; set; } = string.Empty;

        [TextField(13, 2)] public int Recordcode { get; set; }

        [TextField(15, 9)] public string BSN { get; set; } = string.Empty;

        [TextField(24, 25)] public string Gegevenselement { get; set; } = string.Empty;

        [TextField(49, 94)] public string Waarde { get; set; } = string.Empty;

        [TextField(143, 17)] public string Reserve { get; set; } = string.Empty;
    }

    [TextSerializable]
    public class Waarderecord3
    {
        [TextField(1, 12)] public string IndicatiefInzender { get; set; } = string.Empty;

        [TextField(13, 2)] public int Recordcode { get; set; }

        [TextField(15, 9)] public string BSN { get; set; } = string.Empty;

        [TextField(24, 25)] public string Gegevenselement { get; set; } = string.Empty;

        [TextField(49, 88)] public string Waarde { get; set; } = string.Empty;

        [TextField(137, 23)] public string Reserve { get; set; } = string.Empty;
    }

    [TextSerializable]
    public class Waarderecord4
    {
        [TextField(1, 12)] public string IndicatiefInzender { get; set; } = string.Empty;

        [TextField(13, 2)] public int Recordcode { get; set; }

        [TextField(15, 9)] public string BSN { get; set; } = string.Empty;

        [TextField(24, 25)] public string Gegevenselement { get; set; } = string.Empty;

        [TextField(49, 49)] public string Waarde { get; set; } = string.Empty;

        [TextField(98, 62)] public string Reserve { get; set; } = string.Empty;
    }

    [TextSerializable]
    public class Sluitrecord
    {
        [TextField(1, 12)] public string IndicatiefInzender { get; set; } = string.Empty;

        [TextField(13, 2)] public int Recordcode { get; set; }

        [TextField(15, 9)] public int AantalRecords { get; set; }
    }
}