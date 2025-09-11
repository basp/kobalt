using Kobalt.Belastingdienst;
using Microsoft.Extensions.Logging;

var logger = LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
        builder.SetMinimumLevel(LogLevel.Trace);
    })
    .CreateLogger<Program>();

const string path = @"D:\temp\BD\INL_VINLBUR010_goed_20240516.txt";
using var stream = File.OpenRead(path);
var parser = new Parser(logger);
parser.Parse(stream);

internal class Parser : BankrekeningenParser
{
    public Parser(ILogger logger) : base(logger)
    {
        
    }
}
