using Jiggswap.Application.Trades.Dtos;
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

        public JiggswapNewTradeEmail(IConfiguration config) : base(config)
        {
        }

        public async Task<Response> SendNewTradeEmail(string recipientEmail, TradeDetailsDto tradeDetails)
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
                    puzRcpImageUrl: $"{_sendGridBaseUrl}/image/{tradeDetails.RequestedPuzzleImageId}",

                    puzInitName: tradeDetails.InitiatorPuzzleTitle,
                    puzInitPcs: tradeDetails.InitiatorPuzzleNumPieces,
                    puzInitBrand: tradeDetails.InitiatorPuzzleBrand,
                    puzInitImageUrl: $"{_sendGridBaseUrl}/image/{tradeDetails.InitiatorPuzzleImageId}",

                    tradeAcceptUrl: $"{_sendGridBaseUrl}/trade/view?t={tradeDetails.TradeId}&act=accept",
                    tradeDeclineUrl: $"{_sendGridBaseUrl}/trade/view?t={tradeDetails.TradeId}&act=decline",
                    tradeViewUrl: $"{_sendGridBaseUrl}/trade/view?t={tradeDetails.TradeId}"
                )
            });
        }
    }
}