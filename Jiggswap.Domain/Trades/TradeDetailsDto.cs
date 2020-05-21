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

        public string InitiatorCity { get; set; }
        public string RequestedCity { get; set; }

        public string InitiatorZip { get; set; }
        public string RequestedZip { get; set; }

        public string InitiatorStreet { get; set; }
        public string RequestedStreet { get; set; }

        /* Puzzle Details */
        public string InitiatorPuzzleTitle { get; set; }
        public string RequestedPuzzleTitle { get; set; }

        public string InitiatorPuzzleBrand { get; set; }
        public string RequestedPuzzleBrand { get; set; }

        public string InitiatorPuzzleTags { get; set; }
        public string RequestedPuzzleTags { get; set; }

        public string InitiatorPuzzleImageId { get; set; }
        public string RequestedPuzzleImageId { get; set; }

        public string InitiatorPuzzleNumPieces { get; set; }
        public string RequestedPuzzleNumPieces { get; set; }

        public string RequestedPuzzleNumPiecesMissing { get; set; }
        public string InitiatorPuzzleNumPiecesMissing { get; set; }

        /* Shipment / Status Details */
        public string InitiatorPuzzleStatus { get; set; }
        public string RequestedPuzzleStatus { get; set; }

        public string InitiatorPuzzleShippedVia { get; set; }
        public string RequestedPuzzleShippedVia { get; set; }

        public string InitiatorPuzzleShippedTrackingNo { get; set; }
        public string RequestedPuzzleShippedTrackingNo { get; set; }
    }
}