using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Jiggswap.Application.Emails;
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
        private readonly IJiggswapForgotPasswordEmail _notifier;

        public ForgotPasswordNotificationEmailHandler(IJiggswapForgotPasswordEmail notifier)
        {
            _notifier = notifier;
        }

        public async Task Handle(ForgotPasswordNotification request, CancellationToken cancellationToken)
        {
            await _notifier.SendForgotPasswordEmail(request.Email, request.Token);
        }
    }
}