using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.Extensions.Logging;

namespace Kobalt.Belastingdienst;

public class AlleenverdienersParser
{
    private readonly ILogger logger;
    
    public AlleenverdienersParser(ILogger logger)
    {
        this.logger = logger;
    }

    public IEnumerable<AvdRecord> Parse(Stream stream)
    {
        var config = new CsvConfiguration(CultureInfo.InstalledUICulture)
        {
            IgnoreBlankLines = true,
            Delimiter = ";",
            MissingFieldFound = null,
        };

        var reader = new StreamReader(stream);
        var csv = new CsvReader(reader, config);
        return csv.GetRecords<AvdRecord>();
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class AvdRecord
    {
        [Index(0)]
        public string? BSN { get; set; }
        
        [Index(1)]
        public string? Gemeentecode { get; set; }
    }
}