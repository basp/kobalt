using Microsoft.Extensions.Logging;
using Setl;
using Setl.Operations;

namespace Kobalt.Belastingdienst;

public class ParseWaardeOperation : AbstractOperation
{
    public ParseWaardeOperation(ILogger logger) : base(logger)
    {
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        throw new NotImplementedException();
    }
}