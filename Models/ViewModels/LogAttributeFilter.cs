namespace bretts_services.Models.ViewModels;

/// <summary>Represents one condition applied to a structured property in a log event.</summary>
public record LogAttributeFilter
{
    /// <summary>Gets or sets the property name stored in the Serilog event JSON.</summary>
    public string Attribute { get; set; } = string.Empty;

    /// <summary>Gets or sets the comparison to apply.</summary>
    [EnumDataType(typeof(LogFilterOperator))]
    public LogFilterOperator Operator { get; set; }

    /// <summary>Gets or sets the comparison value. Existence operators do not use it.</summary>
    public string? Value { get; set; }
}
