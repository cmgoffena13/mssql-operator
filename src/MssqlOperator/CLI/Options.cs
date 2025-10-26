using CommandLine;

namespace MssqlOperator.CLI;

[Verb("database", HelpText = "Database information")]
public class DatabaseOptions
{
    [Option('n', "name", HelpText = "Specific database name to show details for")]
    public string? Database { get; set; }

    [Option('a', "all", HelpText = "Show all databases without selection menu")]
    public bool ShowAll { get; set; }
}
