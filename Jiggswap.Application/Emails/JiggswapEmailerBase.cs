using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jiggswap.Application.Emails
{
    public class JiggswapHtmlEmail
    {
        public string Subject { get; set; }

        public string HtmlContent { get; set; }

        public string PlainContent { get; set; }

        public EmailAddress ToEmail { get; set; }
    }

    public class JiggswapTemplateEmail
    {
        public string TemplateId { get; set; }

        public object TemplateData { get; set; }

        public EmailAddress ToEmail { get; set; }
    }

    public interface IJiggswapEmailerBase
    {
    }

    public class JiggswapEmailerBase : IJiggswapEmailerBase
    {
        private readonly string _sendGridApiKey;
        private readonly string _sendGridFromEmail;
        private readonly string _sendGridFromName;
        protected readonly string _sendGridBaseUrl;

        public JiggswapEmailerBase(IConfiguration config)
        {
            _sendGridApiKey = config["Notifications:SendGridApiKey"];
            _sendGridFromEmail = config["Notifications:FromEmail"];
            _sendGridFromName = config["Notifications:FromName"];
            _sendGridBaseUrl = config["Notifications:BaseUrl"];
        }

        protected Task<Response> SendHtmlEmail(JiggswapHtmlEmail notification)
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

        protected Task<Response> SendTemplateEmail(JiggswapTemplateEmail notification)
        {
            var sendGridClient = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress(_sendGridFromEmail, _sendGridFromName);

            var msg = MailHelper.CreateSingleTemplateEmail(from, notification.ToEmail, notification.TemplateId, notification.TemplateData);
            return sendGridClient.SendEmailAsync(msg);
        }
    }
}