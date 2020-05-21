using MediatR;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jiggswap.Application.Emails
{
    public interface IJiggswapForgotPasswordEmail
    {
        public Task<Response> SendForgotPasswordEmail(string email, string token);
    }

    public class JiggswapForgotPasswordEmail : JiggswapEmailerBase, IJiggswapForgotPasswordEmail
    {
        public JiggswapForgotPasswordEmail(IConfiguration config) : base(config)
        {
        }

        public async Task<Response> SendForgotPasswordEmail(string email, string token)
        {
            var resetUrl = $"{_sendGridBaseWebUrl}/reset-password?key={token}";

            return await SendHtmlEmail(new JiggswapHtmlEmail
            {
                Subject = "Jiggswap - Password Reset Instructions",
                ToEmail = new EmailAddress(email),
                ReplyTo = new EmailAddress("help@jiggswap.com"),
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