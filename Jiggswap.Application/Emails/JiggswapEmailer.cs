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
using Jiggswap.RazorViewEngine;
using Jiggswap.RazorViewEngine.ViewModels;
using Jiggswap.Domain.Trades;
using Jiggswap.RazorViewEngine.ViewModels.Trades;
using Jiggswap.RazorViewEngine.ViewModels.Feedback;
using Jiggswap.RazorViewEngine.ViewModels.Passwords;

namespace Jiggswap.Application.Emails
{
    public class JiggswapRenderedEmail<TModel>
    {
        public JiggswapRenderedEmail(TModel model)
        {
            Model = model;
        }

        public string Subject { get; set; }

        public EmailAddress ToEmail { get; set; }

        public EmailAddress ReplyTo { get; set; }

        public TModel Model { get; }
    }

    public interface IJiggswapEmailer
    {
        Task<Response> SendNewTradeEmail(string recipientEmail, TradeDetailsDto tradeDetails);

        Task<Response> SendAcceptedTradeEmail(string recipientEmail, TradeDetailsDto tradeDetails);

        Task<Response> SendContactEmail(string name, string email, string comment);

        Task<Response> SendForgotPasswordEmail(string email, string token);
    }

    public class JiggswapEmailer : IJiggswapEmailer
    {
        private readonly string _sendGridApiKey;
        private readonly string _sendGridFromEmail;
        private readonly string _sendGridFromName;
        protected readonly string _sendGridBaseWebUrl;
        protected readonly string _sendGridBaseApiUrl;

        private readonly bool _useRealEmails;

        private readonly IJiggswapRazorViewRenderer _razorRenderer;

        public JiggswapEmailer(IConfiguration config, IJiggswapRazorViewRenderer razorRenderer)
        {
            _sendGridApiKey = config["Notifications:SendGridApiKey"];
            _sendGridFromEmail = config["Notifications:FromEmail"];
            _sendGridFromName = config["Notifications:FromName"];
            _sendGridBaseWebUrl = config["Notifications:BaseWebUrl"];
            _sendGridBaseApiUrl = config["Notifications:BaseApiUrl"];

            _useRealEmails = bool.Parse(config["Notifications:UseRealEmails"]);
            _razorRenderer = razorRenderer;
        }

        private async Task<Response> SendRazorRenderedEmail<TModel>(JiggswapRenderedEmail<TModel> notification)
            where TModel : JiggswapEmailViewModelBase
        {
            var sendGridClient = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress(_sendGridFromEmail, _sendGridFromName);

            var html = await _razorRenderer.RenderViewToStringAsync(notification.Model.RazorViewPath, notification.Model);

            var msg = MailHelper.CreateSingleEmail(
                from,
                notification.ToEmail,
                notification.Subject,
                notification.Model.GetPlainContent(),
                html);

            if (notification.ReplyTo != null)
            {
                msg.SetReplyTo(notification.ReplyTo);
            }

            if (_useRealEmails)
            {
                return await sendGridClient.SendEmailAsync(msg);
            }

            SendTestEmail(msg);

            return await Task.FromResult(new Response(HttpStatusCode.OK, null, null));
        }

        private void SendTestEmail(SendGridMessage msg)
        {
            var testMessage = new MimeMessage();

            testMessage.From.Add(new MailboxAddress(msg.From.Email));

            testMessage.To.Add(new MailboxAddress("test@jiggswap.com"));

            if (!string.IsNullOrEmpty(msg.ReplyTo?.Email))
            {
                testMessage.ReplyTo.Add(new MailboxAddress(msg.ReplyTo.Email));
            }

            testMessage.Subject = msg.Personalizations[0].Subject;

            testMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = msg.Contents[1].Value
            };

            using var client = new SmtpClient
            {
                ServerCertificateValidationCallback = (_, __, ___, ____) => true
            };

            client.Connect("127.0.0.1", 25, false);
            client.Send(testMessage);
            client.Disconnect(true);
        }

        public async Task<Response> SendNewTradeEmail(string recipientEmail, TradeDetailsDto tradeDetails)
        {
            var viewModel = new NewTradeEmailViewModel(_sendGridBaseApiUrl, _sendGridBaseWebUrl, tradeDetails);

            return await SendRazorRenderedEmail(new JiggswapRenderedEmail<NewTradeEmailViewModel>(viewModel)
            {
                Subject = $"Jiggswap - New Trade Request",
                ToEmail = new EmailAddress(recipientEmail),
            });
        }

        public async Task<Response> SendAcceptedTradeEmail(string recipientEmail, TradeDetailsDto tradeDetails)
        {
            var viewModel = new AcceptedTradeEmailViewModel(_sendGridBaseApiUrl, _sendGridBaseWebUrl, tradeDetails);

            return await SendRazorRenderedEmail(new JiggswapRenderedEmail<AcceptedTradeEmailViewModel>(viewModel)
            {
                Subject = $"Jiggswap - Trade Accepted!",
                ToEmail = new EmailAddress(recipientEmail),
            });
        }

        public async Task<Response> SendContactEmail(string name, string email, string comment)
        {
            var viewModel = new ContactEmailViewModel
            {
                Name = name,
                Email = email,
                Comment = comment
            };

            return await SendRazorRenderedEmail(new JiggswapRenderedEmail<ContactEmailViewModel>(viewModel)
            {
                Subject = $"Jiggswap - Comment from {name}",
                ToEmail = new EmailAddress("comments@jiggswap.com"),
                ReplyTo = new EmailAddress(email)
            });
        }

        public async Task<Response> SendForgotPasswordEmail(string email, string token)
        {
            var resetUrl = $"{_sendGridBaseWebUrl}/reset-password?key={token}";

            var viewModel = new ForgotPasswordEmailViewModel { ResetUrl = resetUrl };

            return await SendRazorRenderedEmail(new JiggswapRenderedEmail<ForgotPasswordEmailViewModel>(viewModel)
            {
                Subject = $"Jiggswap - Password Reset Instructions",
                ToEmail = new EmailAddress(email),
            });
        }
    }
}