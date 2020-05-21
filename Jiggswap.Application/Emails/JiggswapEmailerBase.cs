using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using System.Net;

namespace Jiggswap.Application.Emails
{
    public class JiggswapHtmlEmail
    {
        public string Subject { get; set; }

        public string HtmlContent { get; set; }

        public string PlainContent { get; set; }

        public EmailAddress ToEmail { get; set; }

        public EmailAddress ReplyTo { get; set; }
    }

    public class JiggswapTemplateEmail
    {
        public string TemplateId { get; set; }

        public object TemplateData { get; set; }

        public EmailAddress ToEmail { get; set; }

        public EmailAddress ReplyTo { get; set; }
    }

    public class JiggswapRenderedEmail
    {
        public string Subject { get; set; }

        public string HtmlContent { get; set; }

        public string PlainContent { get; set; }

        public EmailAddress ToEmail { get; set; }

        public EmailAddress ReplyTo { get; set; }
    }

    public interface IJiggswapEmailerBase
    {
    }

    public class JiggswapEmailerBase : IJiggswapEmailerBase
    {
        private readonly string _sendGridApiKey;
        private readonly string _sendGridFromEmail;
        private readonly string _sendGridFromName;
        protected readonly string _sendGridBaseWebUrl;
        protected readonly string _sendGridBaseApiUrl;

        private readonly bool _useRealEmails;

        public JiggswapEmailerBase(IConfiguration config)
        {
            _sendGridApiKey = config["Notifications:SendGridApiKey"];
            _sendGridFromEmail = config["Notifications:FromEmail"];
            _sendGridFromName = config["Notifications:FromName"];
            _sendGridBaseWebUrl = config["Notifications:BaseWebUrl"];
            _sendGridBaseApiUrl = config["Notifications:BaseApiUrl"];

            _useRealEmails = bool.Parse(config["Notifications:UseRealEmails"]);
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

            if (notification.ReplyTo != null)
            {
                msg.SetReplyTo(notification.ReplyTo);
            }

            if (_useRealEmails)
            {
                return sendGridClient.SendEmailAsync(msg);
            }

            Console.WriteLine("Email not sent due to configuration:");
            Console.WriteLine(html);

            SendTestEmail(msg);

            return Task.FromResult(new Response(HttpStatusCode.OK, null, null));
        }

        protected Task<Response> SendTemplateEmail(JiggswapTemplateEmail notification)
        {
            var sendGridClient = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress(_sendGridFromEmail, _sendGridFromName);

            var msg = MailHelper.CreateSingleTemplateEmail(from, notification.ToEmail, notification.TemplateId, notification.TemplateData);

            if (notification.ReplyTo != null)
            {
                msg.SetReplyTo(notification.ReplyTo);
            }

            if (_useRealEmails)
            {
                return sendGridClient.SendEmailAsync(msg);
            }

            Console.WriteLine("Template email not sent due to config");

            return Task.FromResult(new Response(HttpStatusCode.OK, null, null));
        }

        protected Task<Response> SendRazorRenderedEmail(JiggswapRenderedEmail notification)
        {
            var sendGridClient = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress(_sendGridFromEmail, _sendGridFromName);

            var msg = MailHelper.CreateSingleEmail(from, notification.ToEmail, notification.Subject, notification.PlainContent, notification.HtmlContent);

            if (notification.ReplyTo != null)
            {
                msg.SetReplyTo(notification.ReplyTo);
            }

            if (_useRealEmails)
            {
                return sendGridClient.SendEmailAsync(msg);
            }

            SendTestEmail(msg);

            return Task.FromResult(new Response(HttpStatusCode.OK, null, null));
        }

        private void SendTestEmail(SendGridMessage msg)
        {
            var testMessage = new MimeMessage();

            testMessage.From.Add(new MailboxAddress(msg.From.Email));

            testMessage.To.Add(new MailboxAddress("test@jiggswap.com"));

            testMessage.Subject = msg.Personalizations[0].Subject;

            testMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = msg.Contents[1].Value
            };

            using var client = new SmtpClient();
            client.ServerCertificateValidationCallback = (_, __, ___, ____) => true;
            client.Connect("127.0.0.1", 25, false);
            client.Send(testMessage);
            client.Disconnect(true);
        }
    }
}