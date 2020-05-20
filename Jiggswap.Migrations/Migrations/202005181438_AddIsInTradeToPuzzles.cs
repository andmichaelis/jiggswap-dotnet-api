using FluentMigrator;

namespace Jiggswap.Migrations.Migrations
{
    [Migration(202005181438)]
    public class AddIsInTradeToPuzzles : Migration
    {
        public override void Up()
        {
            _ = Create
                .Column("is_in_trade")
                .OnTable("puzzles")
                .AsBoolean()
                .WithDefaultValue(false);
        }

        public override void Down()
        {
            _ = Delete
                .Column("is_in_trade")
                .FromTable("puzzles");
        }
    }
}