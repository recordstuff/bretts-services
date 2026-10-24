namespace bretts_services.Models.ViewModels;

/// <summary>Defines the severity of a log event in the API contract.</summary>
public enum LogEventLevel
{
    Verbose = 0,
    Debug = 1,
    Information = 2,
    Warning = 3,
    Error = 4,
    Fatal = 5,
}
