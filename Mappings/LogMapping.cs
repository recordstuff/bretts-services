using bretts_services.Models.ViewModels;
using Riok.Mapperly.Abstractions;
using LogEntity = bretts_services.Models.Entities.Log;
using SerilogLogEventLevel = Serilog.Events.LogEventLevel;

namespace bretts_services.Mappings;

[Mapper(
    EnumMappingStrategy = EnumMappingStrategy.ByValueCheckDefined,
    RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class LogMapping
{
    [MapProperty(nameof(LogEntity.LogGuid), nameof(LogSummary.Guid))]
    public partial LogSummary ToLogSummary(LogEntity log);

    public partial List<LogSummary> ToLogSummaries(List<LogEntity> logs);

    [MapProperty(nameof(LogEntity.LogGuid), nameof(LogDetail.Guid))]
    public partial LogDetail ToLogDetail(LogEntity log);

    [MapperIgnoreTarget(nameof(LogEntity.Id))]
    [MapperIgnoreTarget(nameof(LogEntity.LogGuid))]
    public partial LogEntity ToLog(LogDetail logDetail);

    [MapperIgnoreTarget(nameof(LogEntity.Id))]
    [MapperIgnoreTarget(nameof(LogEntity.LogGuid))]
    public partial void UpdateLog(LogDetail source, LogEntity target);

    public partial SerilogLogEventLevel ToSerilogLogEventLevel(LogEventLevel level);
}
