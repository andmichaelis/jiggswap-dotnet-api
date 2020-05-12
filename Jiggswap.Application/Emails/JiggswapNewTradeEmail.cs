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
                TemplateData = new
                {
                    userInitName = tradeDetails.InitiatorUsername,
                    userRcpName = tradeDetails.RequestedUsername,

                    puzRcpName = tradeDetails.RequestedPuzzleTitle,
                    puzRcpPcs = tradeDetails.RequestedPuzzlePieces,
                    puzRcpBrand = tradeDetails.RequestedPuzzleBrand,
                    puzRcpImageUrl = $"https://i5.walmartimages.com/asr/a8f5140c-9271-4440-b64b-997240936d19_1.5d9cab66a545eaa83779b9867ea5bbdd.jpeg",

                    puzInitName = tradeDetails.InitiatorPuzzleTitle,
                    puzInitPcs = tradeDetails.InitiatorPuzzlePieces,
                    puzInitBrand = tradeDetails.InitiatorPuzzleBrand,
                    puzInitImageUrl = $"https://i5.walmartimages.com/asr/a8f5140c-9271-4440-b64b-997240936d19_1.5d9cab66a545eaa83779b9867ea5bbdd.jpeg"
                }
            });
        }
    }
}