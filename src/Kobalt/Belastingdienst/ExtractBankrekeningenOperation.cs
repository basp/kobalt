using Microsoft.Extensions.Logging;
using Setl;

namespace Kobalt.Belastingdienst;

public class ExtractBankrekeningenOperation : AbstractOperation
{
    private readonly BankrekeningenParser parser;
    private readonly Stream source;
    
    public ExtractBankrekeningenOperation(
        Stream source,
        ILogger logger) : base(logger)
    {
        this.source = source;
        this.parser = new BankrekeningenParser(logger);
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows) =>
        this.parser.Parse(this.source);
}