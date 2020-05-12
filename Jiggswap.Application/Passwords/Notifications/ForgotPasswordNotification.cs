using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Jiggswap.Notifications;
using Jiggswap.Notifications.Common;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Jiggswap.Application.Passwords.Notifications
{
    public class ForgotPasswordNotification : INotification
    {
        public string Email { get; set; }

        public string Token { get; set; }
    }

    public class ForgotPasswordNotificationEmailHandler : INotificationHandler<ForgotPasswordNotification>
    {
        private readonly IJiggswapNotifier _notifier;

        public ForgotPasswordNotificationEmailHandler(IJiggswapNotifier notifier)
        {
            _notifier = notifier;
        }

        public async Task Handle(ForgotPasswordNotification request, CancellationToken cancellationToken)
        {
            await _notifier.SendForgotPasswordEmail(request.Email, request.Token);
        }
    }
}