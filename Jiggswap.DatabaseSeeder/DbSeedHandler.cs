using Jiggswap.DatabaseSeeder.Seeders;
using Npgsql;
using static Jiggswap.DatabaseSeeder.Seeders.PuzzleSeeder;
using static Jiggswap.DatabaseSeeder.Seeders.UserSeeder;

namespace Jiggswap.DatabaseSeeder
{
    internal class DbSeedHandler
    {
        private UserSeeder _userSeeder;
        private PuzzleSeeder _puzSeeder;
        private TradeSeeder _tradeSeeder;

        public static SeededUser Wheelworks = new SeededUser("wheelworks", "wheelworks@jiggswap.com",
        new SeededUserProfile
        {
            FirstName = "Wheel",
            LastName = "Works",
            City = "New York City",
            State = "NY",
            StreetAddress = "101 First Ave",
            Zip = "10001"
        });

        public static SeededUser Hyperethics = new SeededUser("hyperethics", "hyperethics@jiggswap.com",
        new SeededUserProfile
        {
            FirstName = "Hyper",
            LastName = "Ethics",
            City = "Los Angeles",
            State = "CA",
            StreetAddress = "The Beach",
            Zip = "-1"
        });

        public static SeededUser Doglover = new SeededUser("doglover714", "doglover714@jiggswap.com",
        new SeededUserProfile
        {
            FirstName = "Dog",
            LastName = "Lover",
            StreetAddress = "411 Elm St",
            City = "Dallas",
            State = "TX",
            Zip = "75202"
        });

        internal DbSeedHandler()
        {
            var db = new NpgsqlConnection("User ID=postgres;Password=postgres;Database=postgres;Host=localhost;Port=5432");

            _userSeeder = new UserSeeder(db);
            _puzSeeder = new PuzzleSeeder(db);
            _tradeSeeder = new TradeSeeder(db);
        }

        public void SeedAll()
        {
            _userSeeder.CreateUser(Wheelworks);
            _userSeeder.CreateUser(Hyperethics);
            _userSeeder.CreateUser(Doglover);

            for (var idx = 0; idx < 5; idx++)
            {
                _puzSeeder.CreateRandomPuzzleForUser(Wheelworks);
            }

            for (var idx = 0; idx < 5; idx++)
            {
                _puzSeeder.CreateRandomPuzzleForUser(Hyperethics);
            }

            for (var idx = 0; idx < 3; idx++)
            {
                _puzSeeder.CreateRandomPuzzleForUser(Doglover);
            }

            _tradeSeeder.CreateTrade(Wheelworks.Puzzles[0], Hyperethics.Puzzles[0]);
            _tradeSeeder.CreateTrade(Wheelworks.Puzzles[0], Hyperethics.Puzzles[1]);
            _tradeSeeder.CreateTrade(Wheelworks.Puzzles[0], Doglover.Puzzles[0]);
            _tradeSeeder.CreateTrade(Hyperethics.Puzzles[0], Doglover.Puzzles[1]);
        }
    }
}