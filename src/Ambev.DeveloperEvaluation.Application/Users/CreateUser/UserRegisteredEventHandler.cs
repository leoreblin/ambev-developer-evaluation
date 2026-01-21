using Ambev.DeveloperEvaluation.Domain.Events;
using DnsClient.Internal;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Users.CreateUser;

internal sealed class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
{
    private readonly ILogger<UserRegisteredEventHandler> _logger;

    public UserRegisteredEventHandler(ILogger<UserRegisteredEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "User registered: {UserId}, {Email}, {Name}",
            notification.User.Id, notification.User.Email, notification.User.Username);

        await Task.CompletedTask;
    }
}
