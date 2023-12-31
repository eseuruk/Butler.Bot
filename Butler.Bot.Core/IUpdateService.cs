﻿namespace Butler.Bot.Core;

public interface IUpdateService
{
    Task HandleUpdateAsync(Update update, CancellationToken cancellationToken);
}
