using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jiggswap.Application.Emails
{
    public interface IJiggswapFeedbackEmail
    {
        public Task<Response> SendForgotPasswordEmail(string name, string email, string comment);
    }

    public class JiggswapFeedbackEmail : JiggswapEmailerBase, IJiggswapFeedbackEmail
    {
        public JiggswapFeedbackEmail(IConfiguration config) : base(config)
        {
        }

        public async Task<Response> SendForgotPasswordEmail(string name, string email, string comment)
        {
            return await SendHtmlEmail(new JiggswapHtmlEmail
            {
                Subject = "Jiggswap - Password Reset Instructions",
                ToEmail = new EmailAddress("contact@jiggswap.com"),
                ReplyTo = new EmailAddress(email),
                PlainContent = $"New Message Received from {name} <{email}>: {comment}",
                HtmlContent = $@"<div>
                        {name} <{email}> has sent a message via www.jiggswap.com:
                        <hr />
                            {comment}
                        <hr />
                        If you have any questions, please get in touch with us by replying to this email.
                    </div>"
            });
        }
    }
}