using Microsoft.Extensions.Logging;
using Setl;
using Setl.Operations;

namespace Kobalt.SVB;

public class WWB65PlusAggregationOperation : AbstractOperation
{
    private static class WellKnownRecordcodes
    {
        public const string Bericht = "BER";
        public const string Gemeente = "GEM";
        public const string Detail = "DTR";
        public const string Tellingen = "TPG";
    }

    private Row gemeente = [];
    private List<Row> details = [];
    
    public WWB65PlusAggregationOperation(ILogger logger) : base(logger)
    {
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        foreach (var row in rows)
        {
            var recordcode = row["Recordcode"];
            switch (recordcode)
            {
                case WellKnownRecordcodes.Bericht:
                    break;
                case WellKnownRecordcodes.Gemeente:
                    this.gemeente = row.Clone();
                    break;
                case WellKnownRecordcodes.Detail:
                    this.details.Add(row.Clone());
                    break;
                case WellKnownRecordcodes.Tellingen:
                    var bericht = new Gemeentebericht
                    {
                        Gemeente = this.gemeente,
                        Details = this.details,
                        Tellingen = row.Clone(),
                    };
                    
                    this.gemeente = [];
                    this.details = [];
                    
                    yield return Row.FromObject(bericht);
                    break;
            }
        }
    }

    public class Gemeentebericht
    {
        public Row Gemeente { get; set; } = [];
        public List<Row> Details { get; set; } = [];
        public Row Tellingen { get; set; } = [];
    }
}