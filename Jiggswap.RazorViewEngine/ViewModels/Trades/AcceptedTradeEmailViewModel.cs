using Jiggswap.Domain.Trades;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.RazorViewEngine.ViewModels.Trades
{
    public class AcceptedTradeEmailViewModel : BaseTradeEmailViewModel
    {
        public AcceptedTradeEmailViewModel(string apiUrl, string webUrl, TradeDetailsDto tradeDetails) : base(apiUrl, webUrl, tradeDetails)
        {
            InitiatorPuzzle.HideDetails = true;
            RequestedPuzzle.HideDetails = true;
        }

        public override string RazorViewPath => "/Views/Emails/Trades/AcceptedTradeEmail.cshtml";

        public override string GetPlainContent()
        {
            return "Accepted.";
        }
    }
}