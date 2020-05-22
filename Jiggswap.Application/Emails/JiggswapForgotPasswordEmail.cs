using Jiggswap.RazorViewEngine;
using Jiggswap.RazorViewEngine.ViewModels.Passwords;
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
        public JiggswapForgotPasswordEmail(IConfiguration config, IJiggswapRazorViewRenderer razorRenderer) : base(config, razorRenderer)
        {
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