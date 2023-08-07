using Butler.Bot.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Butler.Bot.SQLite;

public class JoinRequestTable
{
    private readonly SQLiteUserRepositoryOptions options;
    private readonly ILogger<JoinRequestTable> logger;

    public JoinRequestTable(IOptions<SQLiteUserRepositoryOptions> options, ILogger<JoinRequestTable> logger)
    {
        this.options = options.Value;
        this.logger = logger;
    }

    internal Task CreateRecordAsync(JoinRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    internal Task<JoinRequest?> SelectRecordAsync(long userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    internal Task UpdateRecordAsync(JoinRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
