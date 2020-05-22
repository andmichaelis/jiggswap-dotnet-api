using Jiggswap.RazorViewEngine;
using Jiggswap.RazorViewEngine.ViewModels.Feedback;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jiggswap.Application.Emails
{
    public interface IJiggswapContactEmail
    {
        public Task<Response> SendContactEmail(string name, string email, string comment);
    }

    public class JiggswapContactEmail : JiggswapEmailerBase, IJiggswapContactEmail
    {
        public JiggswapContactEmail(IConfiguration config, IJiggswapRazorViewRenderer razorRenderer) : base(config, razorRenderer)
        {
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
    }
}