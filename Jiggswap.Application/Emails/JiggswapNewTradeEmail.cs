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
        private readonly IJiggswapRazorViewRenderer _razorRenderer;

        public static string TemplateId => "d-1d1f5092884d4842b5afa9b0270e04a6";

        public JiggswapNewTradeEmail(IConfiguration config, IJiggswapRazorViewRenderer razorRenderer) : base(config)
        {
            _razorRenderer = razorRenderer;
        }

        public async Task<Response> SendNewTradeEmail(string recipientEmail, TradeDetailsDto tradeDetails)
        {
            var viewModel = new NewTradeEmailViewModel(_sendGridBaseApiUrl, _sendGridBaseWebUrl, tradeDetails);

            var html = await _razorRenderer.RenderViewToStringAsync("/Views/Emails/Trades/NewTradeEmail.cshtml", viewModel);

            return await SendRazorRenderedEmail(new JiggswapRenderedEmail
            {
                HtmlContent = html,
                PlainContent = viewModel.GetPlainContent(),
                Subject = $"Jiggswap - New Trade Request",
                ToEmail = new EmailAddress(recipientEmail)
            });
        }

        public async Task<Response> SendNewTradeEmail(string recipientEmail, TradeDetailsDto tradeDetails, bool sendGrid)
        {
            return await SendTemplateEmail(new JiggswapTemplateEmail
            {
                ToEmail = new EmailAddress(recipientEmail),
                TemplateId = TemplateId,
                TemplateData = (
                    userInitName: tradeDetails.InitiatorUsername,
                    userRcpName: tradeDetails.RequestedUsername,

                    puzRcpName: tradeDetails.RequestedPuzzleTitle,
                    puzRcpPcs: tradeDetails.RequestedPuzzleNumPieces,
                    puzRcpBrand: tradeDetails.RequestedPuzzleBrand,
                    puzRcpImageUrl: $"{_sendGridBaseApiUrl}/image/{tradeDetails.RequestedPuzzleImageId}",

                    puzInitName: tradeDetails.InitiatorPuzzleTitle,
                    puzInitPcs: tradeDetails.InitiatorPuzzleNumPieces,
                    puzInitBrand: tradeDetails.InitiatorPuzzleBrand,
                    puzInitImageUrl: $"{_sendGridBaseApiUrl}/image/{tradeDetails.InitiatorPuzzleImageId}",

                    tradeViewUrl: $"{_sendGridBaseWebUrl}/trades"
                )
            });
        }
    }
}