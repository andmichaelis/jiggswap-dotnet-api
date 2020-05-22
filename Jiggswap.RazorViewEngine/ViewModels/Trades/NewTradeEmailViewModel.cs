using Jiggswap.Domain.Trades;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.RazorViewEngine.ViewModels.Trades
{
    public class NewTradeEmailViewModel : BaseTradeEmailViewModel
    {
        public NewTradeEmailViewModel(string apiUrl, string webUrl, TradeDetailsDto tradeDetails) : base(apiUrl, webUrl, tradeDetails)
        { }

        public override string RazorViewPath => "/Views/Emails/Trades/NewTradeEmail.cshtml";

        public override string GetPlainContent()
        {
            return $"{InitiatorPuzzle.OwnerUsername} wants to trade with you.";
        }
    }
}