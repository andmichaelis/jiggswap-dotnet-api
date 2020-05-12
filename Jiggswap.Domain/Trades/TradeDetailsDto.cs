using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Domain.Trades
{
    public class TradeDetailsDto
    {
        /* Trade Details */
        public Guid TradeId { get; set; }

        public int TradeInternalId { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string Status { get; set; }

        /* User Details */
        public string InitiatorUsername { get; set; }

        public string RequestedUsername { get; set; }

        /* Puzzle Details */
        public string InitiatorPuzzleTitle { get; set; }

        public string RequestedPuzzleTitle { get; set; }

        public string InitiatorPuzzleTags { get; set; }

        public string RequestedPuzzleTags { get; set; }

        public string InitiatorPuzzleImageId { get; set; }

        public string RequestedPuzzleImageId { get; set; }
    }
}