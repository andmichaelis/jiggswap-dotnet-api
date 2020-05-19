using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.DatabaseSeeder.Seeders
{
    internal class TradeSeeder
    {
        private NpgsqlConnection _db;

        internal class SeededTrade
        {
            public int RequestedPuzzleId { get; set; }

            public int RequestedUserId { get; set; }

            public int InitiatorPuzzleId { get; set; }

            public int InitiatorUserId { get; set; }

            public string Status { get; set; }
        }

        public TradeSeeder(NpgsqlConnection db)
        {
            _db = db;
        }

        internal void CreateTrade(PuzzleSeeder.SeededPuzzle initiatorPuzzle, PuzzleSeeder.SeededPuzzle requestedPuzzle)
        {
            var trade = new SeededTrade
            {
                InitiatorUserId = initiatorPuzzle.OwnerId,
                InitiatorPuzzleId = initiatorPuzzle.Id,
                RequestedPuzzleId = requestedPuzzle.Id,
                RequestedUserId = requestedPuzzle.OwnerId
            };

            _db.Query(@"insert into trades
                (initiator_puzzle_id, initiator_user_id,
                 requested_puzzle_id, requested_user_id)
                values
                (@InitiatorPuzzleId, @InitiatorUserId,
                 @RequestedPuzzleId, @RequestedUserId)", trade);

            _db.Query("update puzzles set is_in_trade = true where id = @Id", new { requestedPuzzle.Id });
            _db.Query("update puzzles set is_in_trade = true where id = @Id", new { initiatorPuzzle.Id });
        }
    }
}