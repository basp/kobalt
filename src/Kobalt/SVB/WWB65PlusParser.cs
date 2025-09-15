using System.Text.RegularExpressions;
using Mscribel.FixedWidth;
using Setl;

namespace Kobalt.SVB;

public class WWB65PlusParser
{
    private const string IsBerichtPattern = "^BER";
    private const string IsGemeentePattern = "^GEM";
    private const string IsDetailPattern = "^DTR";
    private const string IsTellingenPattern = "^TPG";

    private static readonly Regex IsBerichtRegex = new(IsBerichtPattern, RegexOptions.Compiled);
    private static readonly Regex IsGemeenteRegex = new(IsGemeentePattern, RegexOptions.Compiled);
    private static readonly Regex IsDetailRegex = new(IsDetailPattern, RegexOptions.Compiled);
    private static readonly Regex IsTellingenRegex = new(IsTellingenPattern, RegexOptions.Compiled);
    
    private static readonly TextSerializer<Berichtrecord> berichtrecordSerializer = new();
    private static readonly TextSerializer<Gemeenterecord> gemeenterecordSerializer = new();
    private static readonly TextSerializer<Detailrecord> detailrecordSerializer = new();
    private static readonly TextSerializer<Tellingenrecord> tellingenrecordSerializer = new();

    private static readonly Dictionary<Regex, Parser> parserConfig;
    
    static WWB65PlusParser()
    {
        parserConfig = new Dictionary<Regex, Parser>
        {
            [WWB65PlusParser.IsBerichtRegex] = new Parser(
                nameof(Berichtrecord), 
                regel => 
                    berichtrecordSerializer.Deserialize(regel)),
            [WWB65PlusParser.IsGemeenteRegex] = new Parser(
                nameof(Gemeenterecord),
                regel =>
                    gemeenterecordSerializer.Deserialize(regel)),
            [WWB65PlusParser.IsDetailRegex] = new Parser(
                nameof(Detailrecord),
                regel =>
                    detailrecordSerializer.Deserialize(regel)),
            [WWB65PlusParser.IsTellingenRegex] = new Parser(
                nameof(Tellingenrecord),
                regel =>
                    tellingenrecordSerializer.Deserialize(regel)),
        };
    }

    public IEnumerable<Row> Parse(Stream stream)
    {
        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }
            
            foreach(var parser in WWB65PlusParser.parserConfig)
            {
                if (parser.Key.IsMatch(line))
                {
                    var record = parser.Value.ParserFunc(line);
                    var row = Row.FromObject(record);
                    row["Tag"] = parser.Value.Tag;
                    yield return row;
                }
            }
        }
    }
    
    private record Parser(string Tag, Func<string, object> ParserFunc);
    
    [TextSerializable]
    private class Berichtrecord
    {
        [TextField(1, 4)] public string Recordcode { get; set; } = string.Empty;

        [TextField(5, 3)] public string Berichttype { get; set; } = string.Empty;

        [TextField(8, 3)] public string FunctieVersie { get; set; } = string.Empty;

        [TextField(11, 35)] public string NaamBericht { get; set; } = string.Empty;

        [TextField(46, 4)] public string CodeSectorLeverancier { get; set; } = string.Empty;

        [TextField(50, 4)] public string CodeSectorAanvrager { get; set; } = string.Empty;

        [TextField(54, 8)] public string DatumAanmaakBericht { get; set; } = string.Empty;

        [TextField(62, 10)] public string ReferentieLevering { get; set; } = string.Empty;
    }

    [TextSerializable]
    public class Gemeenterecord
    {
        [TextField(1, 4)] public string Recordcode { get; set; } = string.Empty;

        [TextField(5, 4)] public string Gemeentecode { get; set; } = string.Empty;

        [TextField(9, 4)] public string Verwerkingsjaar { get; set; } = string.Empty;

        [TextField(13, 2)] public string Verwerkingsmaand { get; set; } = string.Empty;
    }

    [TextSerializable]
    public class Detailrecord
    {
        [TextField(1, 4)] public string Recordcode { get; set; } = string.Empty;

        [TextField(5, 9)] public string SofinummerHp { get; set; } = string.Empty;

        [TextField(14, 25)] public string AchternaamHp { get; set; } = string.Empty;

        [TextField(39, 6)] public string VoorlettersHp { get; set; } = string.Empty;

        [TextField(45, 10)] public string VoorvoegselHp { get; set; } = string.Empty;

        [TextField(55, 28)] public string EersteVoornaamHp { get; set; } = string.Empty;

        [TextField(83, 8)] public string GeboortedatumHp { get; set; } = string.Empty;

        [TextField(91, 40)] public string RekingnummerHp { get; set; } = string.Empty;

        [TextField(131, 9)] public string WWBBedragHp { get; set; } = string.Empty;

        [TextField(140, 4)] public string PostcodeNumeriek { get; set; } = string.Empty;

        [TextField(144, 2)] public string PostcodeLetters { get; set; } = string.Empty;

        [TextField(146, 5)] public string Huisnummer { get; set; } = string.Empty;

        [TextField(151, 4)] public string HuisnummerToevoeging { get; set; } = string.Empty;

        [TextField(155, 24)] public string Straatnaam { get; set; } = string.Empty;

        [TextField(179, 24)] public string Plaatsnaam { get; set; } = string.Empty;

        [TextField(203, 9)] public string SofinummerP { get; set; } = string.Empty;

        [TextField(212, 25)] public string AchternaamP { get; set; } = string.Empty;

        [TextField(237, 6)] public string VoorlettersP { get; set; } = string.Empty;

        [TextField(243, 10)] public string VoorvoegselP { get; set; } = string.Empty;

        [TextField(253, 28)] public string EersteVoornaamP { get; set; } = string.Empty;

        [TextField(281, 8)] public string GeboortedatumP { get; set; } = string.Empty;

        [TextField(289, 40)] public string RekeningnummerP { get; set; } = string.Empty;

        [TextField(329, 9)] public string WWBBedragP { get; set; } = string.Empty;

        [TextField(338, 8)] public string IngangsdatumRecht { get; set; } = string.Empty;

        [TextField(346, 8)] public string EinddatumRecht { get; set; } = string.Empty;

        [TextField(348, 2)] public string WWBNorm { get; set; } = string.Empty;
    }

    [TextSerializable]
    public class Tellingenrecord
    {
        [TextField(1, 4)] public string Recordcode { get; set; } = string.Empty;

        [TextField(5, 4)] public string Gemeentecode { get; set; } = string.Empty;

        [TextField(9, 11)] public string TotaalAantalGerechtigden { get; set; } = string.Empty;

        [TextField(20, 11)] public string TotaalAantalHuishoudens { get; set; } = string.Empty;

        [TextField(31, 11)] public string TotaalWWBBedrag { get; set; } = string.Empty;
    }
}