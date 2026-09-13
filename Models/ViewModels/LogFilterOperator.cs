namespace bretts_services.Models.ViewModels;

/// <summary>Defines a comparison applied to a structured log attribute.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LogFilterOperator
{
    Exists,
    DoesNotExist,
    Equals,
    DoesNotEqual,
    Contains,
    DoesNotContain,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
}
