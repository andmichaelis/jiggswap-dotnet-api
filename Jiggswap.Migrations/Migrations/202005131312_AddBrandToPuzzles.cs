using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Migrations.Migrations
{
    [Migration(202005131312)]
    public class AddBrandToPuzzles : Migration
    {
        public override void Up()
        {
            Alter.Table("puzzles")
                .AddColumn("brand")
                .AsString(50)
                .Nullable();
        }

        public override void Down()
        {
            Delete
                .Column("brand")
                .FromTable("puzzles");
        }
    }
}