using Jiggswap.Application.Trades.Dtos;
using Jiggswap.Domain.Trades;
using Jiggswap.RazorViewEngine;
using Jiggswap.RazorViewEngine.ViewModels.Trades;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Jiggswap.Application.Emails
{
    public interface IJiggswapNewTradeEmail
    {
        public Task<Response> SendNewTradeEmail(string recipientEmail, TradeDetailsDto tradeDetails);
    }

    public class JiggswapNewTradeEmail : JiggswapEmailerBase, IJiggswapNewTradeEmail
    {
        public static string TemplateId => "d-1d1f5092884d4842b5afa9b0270e04a6";

        public JiggswapNewTradeEmail(IConfiguration config, IJiggswapRazorViewRenderer razorRenderer) : base(config, razorRenderer)
        {
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
    }
}