using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Application.Trades.Dtos
{
    public static class TradeStates
    {
        public const string Active = "active";

        public const string Proposed = "proposed";

        public const string Canceled = "canceled";

        public const string Declined = "declined";

        public const string Completed = "completed";
    }
}