using Jiggswap.Domain.Trades;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.RazorViewEngine.ViewModels.Trades
{
    public class TradeEmailPuzzleViewModel
    {
        public string OwnerUsername { get; set; }

        public string Title { get; set; }

        public string NumPieces { get; set; }

        public string NumPiecesMissing { get; set; }

        public string Brand { get; set; }

        public string ImageUrl { get; set; }

        public string ShippingFrom { get; set; }

        public double MaxWidth { get; set; }

        public bool HideDetails { get; set; }
    }
}