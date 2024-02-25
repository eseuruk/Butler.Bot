using Microsoft.AspNetCore.Http.Extensions;

namespace Butler.Bot.AWS;

[ApiController]
public class HealthCheckController : ControllerBase
{
    private readonly IHealthCheckService healthCheckService;

    public HealthCheckController(IHealthCheckService healthCheckService)
    {
        this.healthCheckService = healthCheckService;
    }

    [HttpGet]
    [Route("/health")]
    public async Task<HealthCheckReportDto> CheckHealthAsync(string? componentFilter, CancellationToken cancellationToken)
    {
        var context = ExecutionContextBuilder.CreateContext(Request);
        var fileter = new ComponentFilter(componentFilter);

        var report = await healthCheckService.CheckHealthAsync(context, fileter, cancellationToken);
        
        return CreateResponse(report);
    }

    private HealthCheckReportDto CreateResponse(HealthCheckReport report)
    {
        var response = new HealthCheckReportDto();

        response.ButlerVersion = ButlerVersion.GetCurrent();
        response.Status = report.Status;

        foreach (var reportEntry in report.Entries)
        {
            var responseEntry = new HealthCheckReportDto.Entry
            {
                Status = reportEntry.Value.Status,
                Description = reportEntry.Value.Description
            };

            response.Entries.Add(reportEntry.Key, responseEntry);
        }

        return response;
    }
}
