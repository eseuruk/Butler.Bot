using Microsoft.AspNetCore.Http.Extensions;

namespace Butler.Bot.AWS;

[ApiController]
public class InstallController : ControllerBase
{
    private readonly IInstallService installService;

    public InstallController(IInstallService installService)
    {
        this.installService = installService;
    }

    [HttpGet, HttpPost]
    [Route("/install")]
    public async Task<InstallReportDto> InstallAsync(string? componentFilter, CancellationToken cancellationToken)
    {
        var context = ExecutionContextBuilder.CreateContext(Request);
        var filter = new ComponentFilter(componentFilter);

        var report = await installService.InstallAsync(context, filter, cancellationToken);
        
        return CreateResponse(report);
    }

    [HttpGet, HttpPost]
    [Route("/uninstall")]
    public async Task<InstallReportDto> UninstallAsync(string? componentFilter, CancellationToken cancellationToken)
    {
        var context = ExecutionContextBuilder.CreateContext(Request);
        var filter = new ComponentFilter(componentFilter);

        var report = await installService.UninstallAsync(context, filter, cancellationToken);
        
        return CreateResponse(report);
    }

    private InstallReportDto CreateResponse(InstallReport report)
    {
        var response = new InstallReportDto();

        response.ButlerVersion = ButlerVersion.GetCurrent();
        response.Status = report.Status;

        foreach (var reportEntry in report.Entries)
        {
            var responseEntry = new InstallReportDto.Entry
            {
                Status = reportEntry.Value.Status,
                Description = reportEntry.Value.Description
            };

            response.Entries.Add(reportEntry.Key, responseEntry);
        }

        return response;
    }
}
