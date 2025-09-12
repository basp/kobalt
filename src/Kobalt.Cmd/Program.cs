using Kobalt.Belastingdienst;
using Microsoft.Extensions.Logging;
using Setl;
using Setl.Pipelines;

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

AlleenverdienersExample(logger);

return;

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

#pragma warning disable CS8321 // Local function is declared but never used
static void BankrekeningenExample(ILogger logger)
#pragma warning restore CS8321 // Local function is declared but never used
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
