using System.Net;
using System.Threading.Tasks;
using Jiggswap.Notifications.Exceptions;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Jiggswap.Notifications.Common
{
    public class JiggswapEmailNotification
    {
        public string Subject { get; set; }

        public string HtmlContent { get; set; }

        public string PlainContent { get; set; }

        public EmailAddress ToEmail { get; set; }
    }

    public interface IJiggswapNotifier
    {
        public Task<Response> SendForgotPasswordEmail(string email, string token);
    }

    public class JiggswapNotifier : IJiggswapNotifier
    {
        private readonly string _sendGridApiKey;
        private readonly string _sendGridFromEmail;
        private readonly string _sendGridFromName;
        private readonly string _sendGridBaseUrl;

        public JiggswapNotifier(IConfiguration config)
        {
            _sendGridApiKey = config["Notifications:SendGridApiKey"];
            _sendGridFromEmail = config["Notifications:FromEmail"];
            _sendGridFromName = config["Notifications:FromName"];
            _sendGridBaseUrl = config["Notifications:BaseUrl"];
        }

        private Task<Response> Send(JiggswapEmailNotification notification)
        {
            var sendGridClient = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress(_sendGridFromEmail, _sendGridFromName);
            var html = @$"
                <html>
                    <table width='100%' border='0' cellspacing='0' cellpadding='0'>
                        <tr>
                            <td></td>
                            <td style='text-align:center; width: 70%'>
                            {notification.HtmlContent}
                            </td>
                            <td></td>
                        </tr>
                    </table>
                </html>";

            var msg = MailHelper.CreateSingleEmail(from, notification.ToEmail, notification.Subject, notification.PlainContent, html);
            return sendGridClient.SendEmailAsync(msg);
        }

        public async Task<Response> SendForgotPasswordEmail(string email, string token)
        {
            var resetUrl = $"{_sendGridBaseUrl}/reset-password?key={token}";

            return await Send(new JiggswapEmailNotification
            {
                Subject = "Jiggswap - Password Reset Instructions",
                ToEmail = new EmailAddress(email),
                PlainContent = $"Please use the following link to reset your password: {resetUrl}. If you have any questions, please get in touch with us by replying to this email.",
                HtmlContent = $@"<div>
                        Hello, <br />
                        Please use the following link to reset your password:
                        <hr />
                            <a href='{resetUrl}'>Reset Jiggswap Password</a>
                        <hr />
                        If you have any questions, please get in touch with us by replying to this email.
                    </div>"
            });
        }
    }
}