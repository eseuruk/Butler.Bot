using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Butler.Bot.Core;

public abstract class UpdateHandlerBase
{
    private readonly IButlerBot butler;
    private readonly IUserRepository userRepository;
    private readonly ILogger logger;

    public UpdateHandlerBase(IButlerBot butler, IUserRepository userRepository, ILogger logger)
    {
        this.butler = butler;
        this.userRepository = userRepository;
        this.logger = logger;
    }

    protected IButlerBot Butler { get { return butler; } }

    protected IUserRepository UserRepository { get { return userRepository; } }

    protected ILogger Logger { get { return logger; } }

    public abstract Task<bool> TryHandleUpdateAsync(Update update, CancellationToken cancellationToken);
}
