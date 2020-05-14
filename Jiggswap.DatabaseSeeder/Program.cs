namespace Jiggswap.DatabaseSeeder
{
    public class Program
    {
        public static void Main()
        {
            var handler = new DbSeedHandler();

            handler.SeedAll();
        }
    }
}