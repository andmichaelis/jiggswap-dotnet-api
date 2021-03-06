﻿using Jiggswap.Domain.Trades;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.RazorViewEngine.ViewModels.Trades
{
    public abstract class BaseTradeEmailViewModel : JiggswapEmailViewModelBase
    {
        protected BaseTradeEmailViewModel(string apiUrl, string webUrl, TradeDetailsDto tradeDetails)
        {
            InitiatorPuzzle = new TradeEmailPuzzleViewModel
            {
                OwnerUsername = tradeDetails.InitiatorUsername,
                Brand = tradeDetails.InitiatorPuzzleBrand,
                ImageUrl = tradeDetails.InitiatorPuzzleImageUrl,
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
                ImageUrl = tradeDetails.RequestedPuzzleImageUrl,
                NumPieces = tradeDetails.RequestedPuzzleNumPieces,
                NumPiecesMissing = tradeDetails.RequestedPuzzleNumPiecesMissing,
                Title = tradeDetails.RequestedPuzzleTitle,
                ShippingFrom = tradeDetails.RequestedCity,
                MaxWidth = 288
            };

            ViewUrl = $"{webUrl}/trades";
        }

        public TradeEmailPuzzleViewModel InitiatorPuzzle { get; protected set; }

        public TradeEmailPuzzleViewModel RequestedPuzzle { get; protected set; }

        public string ViewUrl { get; protected set; }
    }
}