using Kobalt.Belastingdienst;
using Mscribel.FixedWidth;

namespace Kobalt.Tests;

public class BankrekeningenParserTest
{
    [Fact]
    public void TestVoorlooprecord()
    {
        const string voorlooprecord =
             "XXXXX012345600FOOBARFOOZ2021FOO BAR               ADRES1              1111AA DUMMY        12511";
        
        var serializer = new TextSerializer<BankrekeningenParser.Voorlooprecord>();
        var actual = serializer.Deserialize(voorlooprecord);
        
        Assert.Equal("XXXXX0123456", actual.IndicatiefInzender);
        Assert.Equal(0, actual.Recordcode);
        Assert.Equal("FOOBARFOOZ", actual.Afnametype);
        Assert.Equal(2021, actual.Belastingjaar);
        Assert.Equal("FOO BAR", actual.Naam);
        Assert.Equal("ADRES1", actual.Adres);
        Assert.Equal("1111AA DUMMY", actual.PostcodeEnWoonplaats);
        Assert.Equal(12, actual.Mediumcode);
        Assert.Equal('5', actual.Density);
        Assert.Equal('1', actual.Characterset);
        Assert.Equal('1', actual.Labelcode);
    }

    [Fact]
    public void TestAlgemeenRecord()
    {
        const string algemeenrecord =
            "XXXXX01234561000003040508081941BEESTJE                                          BM           WAGNSTRAAT               99       1046VDTESTDORP                ";
        
        var serializer = new TextSerializer<BankrekeningenParser.AlgemeenRecord>();
        var actual = serializer.Deserialize(algemeenrecord);
        
        Assert.Equal("XXXXX0123456", actual.IndicatiefInzender);
        Assert.Equal(10, actual.Recordcode);
        Assert.Equal("000030405", actual.BSN);
        Assert.Equal("BEESTJE", actual.Naam);
        Assert.Equal("BM", actual.Voorletters);
        Assert.Equal("WAGNSTRAAT", actual.Straatnaam);
        Assert.Equal("99", actual.Huisnummer);
    }

    [Fact]
    public void TestWaarderecord1()
    {
        const string waarderecord1 =
            "XXXXX012345620164784974GEEN GEGEVENS            20231201                                         0                                                       ";

        var serializer = new TextSerializer<BankrekeningenParser.Waarderecord1>();
        var actual = serializer.Deserialize(waarderecord1);
        
        Assert.Equal("XXXXX0123456", actual.IndicatiefInzender);
        Assert.Equal(20, actual.Recordcode);
        Assert.Equal("164784974", actual.BSN);
        Assert.Equal("GEEN GEGEVENS", actual.Rubriek);
        Assert.Equal("20231201", actual.Waarde);
        Assert.Equal(0, actual.Controlecode);
    }
    
    [Fact]
    public void TestWaarderecord2()
    {
        const string waarderecord2 =
            "XXXXX012345620000030405REKENINGNR/BANKCODE 001  NL00AEGO0417842279                Bankrekening                                       009549237                 ";

        var serializer = new TextSerializer<BankrekeningenParser.Waarderecord2>();
        var actual = serializer.Deserialize(waarderecord2);
        
        Assert.Equal("XXXXX0123456", actual.IndicatiefInzender);
        Assert.Equal(20, actual.Recordcode);
        Assert.Equal("000030405", actual.BSN);
        Assert.Equal("REKENINGNR/BANKCODE 001", actual.Gegevenselement);
        Assert.Equal(
            "NL00AEGO0417842279                Bankrekening                                       009549237",
            actual.Waarde);
    }
    
    [Fact]
    public void TestWaarderecord3()
    {
        const string waarderecord3 =
            "XXXXX012345620000030405NAAM REKENINGHOUDER 001  BM                BEESTJE                                                                                      ";
        
        var serializer = new TextSerializer<BankrekeningenParser.Waarderecord3>();
        var actual = serializer.Deserialize(waarderecord3);
        
        Assert.Equal("XXXXX0123456", actual.IndicatiefInzender);
        Assert.Equal(20, actual.Recordcode);
        Assert.Equal("000030405", actual.BSN);
        Assert.Equal("NAAM REKENINGHOUDER 001", actual.Gegevenselement);
    }
    
    [Fact]
    public void TestWaarderecord4()
    {
        const string waarderecord4 =
            "XXXXX012345620000050106RENTE- EN SALDOBEDRAG 0020000000000000023D 0000000000000100{ 10012022                                                                   ";
        
        var serializer = new TextSerializer<BankrekeningenParser.Waarderecord4>();
        var actual = serializer.Deserialize(waarderecord4);
        
        Assert.Equal("XXXXX0123456", actual.IndicatiefInzender);
        Assert.Equal(20, actual.Recordcode);
        Assert.Equal("000050106", actual.BSN);
        Assert.Equal("RENTE- EN SALDOBEDRAG 002", actual.Gegevenselement);
        Assert.Equal("0000000000000023D 0000000000000100{ 10012022", actual.Waarde);
    }

    [Fact]
    public void TestSluitrecord()
    {
        const string sluitrecord =
            "XXXXX012345601123                                                                                                                                              ";
        
        var serializer = new TextSerializer<BankrekeningenParser.Sluitrecord>();
        var actual = serializer.Deserialize(sluitrecord);
        
        Assert.Equal("XXXXX0123456", actual.IndicatiefInzender);
        Assert.Equal(1, actual.Recordcode);
        Assert.Equal(123, actual.AantalRecords);
    }
}
