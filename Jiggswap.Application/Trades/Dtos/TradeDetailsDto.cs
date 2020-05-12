using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Application.Trades.Dtos
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

        public string InitiatorPuzzleBrand { get; set; }
        public string RequestedPuzzleBrand { get; set; }

        public string InitiatorPuzzlePieces { get; set; }
        public string RequestedPuzzlePieces { get; set; }

        public string InitiatedPuzzlePiecesMissing { get; set; }
        public string RequestedPuzzlePiecesMissing { get; set; }
    }
}