using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Migrations.Migrations
{
    [Migration(202005181438)]
    public class AddIsInTradeToPuzzles : Migration
    {
        public override void Up()
        {
            Create
                .Column("is_in_trade")
                .OnTable("puzzles")
                .AsBoolean()
                .WithDefaultValue(false);
        }

        public override void Down()
        {
            Delete
                .Column("is_in_trade")
                .FromTable("puzzles");
        }
    }
}