using bretts_services.Models.Entities;

namespace bretts_services.Services;

public class DeepHealthCheck : IHealthCheck
{
    private readonly BrettsAppContext _brettsAppContext;

    public DeepHealthCheck(BrettsAppContext brettsAppContext)
    {
        _brettsAppContext = brettsAppContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var role = await _brettsAppContext.Roles.AsNoTracking().FirstOrDefaultAsync();

        if (role is null)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, "Broken");
        }

        return HealthCheckResult.Healthy("Healthy");
    }
}
