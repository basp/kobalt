using Kobalt.Belastingdienst;
using Kobalt.SVB;
using Microsoft.Extensions.Logging;
using Setl;
using Setl.Operations;
using Setl.Pipelines;
#pragma warning disable CS8321 // Local function is declared but never used

var logger = LoggerFactory.Create(builder =>
    {
        builder.AddSimpleConsole(cfg =>
        {
            cfg.TimestampFormat = "[HH:mm:ss] ";
            cfg.SingleLine = true;
        });
        builder.SetMinimumLevel(LogLevel.Trace);
    })
    .CreateLogger<Program>();

var executor = new SingleThreadedPipelineExecutor(logger);

const string path = @"D:\temp\SVB\SVBWWB65PLUS00002_3.txt";

var extract = new SimpleExtract(() =>
    {
        var parser = new WWB65PlusParser();
        var stream = File.OpenRead(path);
        return parser.Parse(stream);
    },
    logger);

var aggregate = new WWB65PlusAggregationOperation(logger);

var load = new SimpleTransform(row =>
    {
        var bericht = row.ToObject<WWB65PlusAggregationOperation.Gemeentebericht>();
        var gemeente = bericht.Gemeente.ToObject<WWB65PlusParser.Gemeenterecord>();
        var details = 
            bericht.Details
                .Select(x => x.ToObject<WWB65PlusParser.Detailrecord>())
                .ToList();
        var tellingen = bericht.Tellingen.ToObject<WWB65PlusParser.Tellingenrecord>();
        logger.LogInformation(
            "{Recordcode}: {Gemeente}", 
            gemeente.Recordcode, 
            gemeente.Gemeentecode);
        logger.LogInformation("DTR: {Count}", details.Count);
        logger.LogInformation(
            "{Recordcode}: {Gemeente} (Huishoudens: {Huishoudens}, Gerechtigden: {Gerechtigden})", 
            tellingen.Recordcode,
            tellingen.Gemeentecode,
            tellingen.TotaalAantalHuishoudens,
            tellingen.TotaalAantalGerechtigden);
        return row;
    },
    logger);

var process = new SimpleProcess(init =>
    {
        init.Register(extract);
        init.Register(aggregate);
        init.Register(load);
    }, 
    logger,
    executor);

process.Execute();

return;

static void WWB65PlusExample(ILogger logger)
{
    const string path = @"D:\temp\SVB\SVBWWB65PLUS00002_3.txt";
    var parser = new WWB65PlusParser();
    var stream = File.OpenRead(path);
    var records = parser.Parse(stream).ToList();
    foreach (var record in records)
    {
        // logger.LogInformation("Record: {Record}", record);
        Console.WriteLine(
            "[Recordcode] {0}", 
            record["Recordcode"]);
    }
}

static void AlleenverdienersExample(ILogger logger)
{
    const string path = @"D:\temp\BD\avd_corrupt.csv";
    var parser = new AlleenverdienersParser(logger);
    var stream = File.OpenRead(path);
    var records = parser.Parse(stream);
    foreach (var record in records)
    {
        // If we try to log with logger here we get some strange behavior.
        // Looks like the stream is cut off early and we don't get all the output.
        // Outputting with Console.WriteLine works fine.
        Console.WriteLine($"[BSN {record.BSN}] [Gemeentecode {record.Gemeentecode}]");
    }   
}

static void BankrekeningenExample(ILogger logger)
{
    const string path = @"D:\temp\BD\INL_VINLBUR010_goed_20240516.txt";
    var executor = new SingleThreadedPipelineExecutor(logger);
    var stream = File.OpenRead(path);
    var process = new SimpleProcess(
        init =>
        {
            init.Register(new ExtractBankrekeningenOperation(stream, logger));
        }, 
        logger, 
        executor);
    process.Execute();    
}
