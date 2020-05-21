using Jiggswap.Domain.Trades;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.RazorViewEngine.Models.Trades
{
    public class NewTradeEmailPuzzleViewModel
    {
        public string OwnerUsername { get; set; }

        public string Title { get; set; }

        public string NumPieces { get; set; }

        public string Brand { get; set; }

        public string ImageUrl { get; set; }
    }

    public class NewTradeEmailViewModel
    {
        public NewTradeEmailViewModel(string apiUrl, string webUrl, TradeDetailsDto tradeDetails)
        {
            InitiatorPuzzle = new NewTradeEmailPuzzleViewModel
            {
                OwnerUsername = tradeDetails.InitiatorUsername,
                Brand = tradeDetails.InitiatorPuzzleBrand,
                ImageUrl = $"{apiUrl}/image/{tradeDetails.InitiatorPuzzleImageId}",
                NumPieces = tradeDetails.InitiatorPuzzleNumPieces,
                Title = tradeDetails.InitiatorPuzzleTitle
            };

            RequestedPuzzle = new NewTradeEmailPuzzleViewModel
            {
                OwnerUsername = tradeDetails.RequestedUsername,
                Brand = tradeDetails.RequestedPuzzleBrand,
                ImageUrl = $"{apiUrl}/image/{tradeDetails.RequestedPuzzleImageId}",
                NumPieces = tradeDetails.RequestedPuzzleNumPieces,
                Title = tradeDetails.RequestedPuzzleTitle
            };
        }

        public NewTradeEmailPuzzleViewModel InitiatorPuzzle { get; set; }

        public NewTradeEmailPuzzleViewModel RequestedPuzzle { get; set; }

        public string GetPlainContent()
        {
            return $"{InitiatorPuzzle.OwnerUsername} wants to trade with you.";
        }
    }
}