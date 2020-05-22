﻿using Jiggswap.Domain.Trades;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.RazorViewEngine.ViewModels.Trades
{
    public class NewTradeEmailViewModel : JiggswapEmailViewModelBase
    {
        public NewTradeEmailViewModel(string apiUrl, string webUrl, TradeDetailsDto tradeDetails)
        {
            InitiatorPuzzle = new TradeEmailPuzzleViewModel
            {
                OwnerUsername = tradeDetails.InitiatorUsername,
                Brand = tradeDetails.InitiatorPuzzleBrand,
                ImageUrl = $"{apiUrl}/image/{tradeDetails.InitiatorPuzzleImageId}",
                NumPieces = tradeDetails.InitiatorPuzzleNumPieces,
                NumPiecesMissing = tradeDetails.InitiatorPuzzleNumPiecesMissing,
                Title = tradeDetails.InitiatorPuzzleTitle,
                ShippingFrom = tradeDetails.InitiatorCity,
                MaxWidth = 288
            };

            RequestedPuzzle = new TradeEmailPuzzleViewModel
            {
                OwnerUsername = tradeDetails.RequestedUsername,
                Brand = tradeDetails.RequestedPuzzleBrand,
                ImageUrl = $"{apiUrl}/image/{tradeDetails.RequestedPuzzleImageId}",
                NumPieces = tradeDetails.RequestedPuzzleNumPieces,
                NumPiecesMissing = tradeDetails.RequestedPuzzleNumPiecesMissing,
                Title = tradeDetails.RequestedPuzzleTitle,
                ShippingFrom = tradeDetails.RequestedCity,
                MaxWidth = 288
            };

            ViewUrl = $"{webUrl}/trades";
        }

        public TradeEmailPuzzleViewModel InitiatorPuzzle { get; set; }

        public TradeEmailPuzzleViewModel RequestedPuzzle { get; set; }

        public string ViewUrl { get; set; }

        public override string RazorViewPath => "/Views/Emails/Trades/NewTradeEmail.cshtml";

        public override string GetPlainContent()
        {
            return $"{InitiatorPuzzle.OwnerUsername} wants to trade with you.";
        }
    }
}