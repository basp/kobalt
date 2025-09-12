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