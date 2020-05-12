using System;
using System.Collections.Generic;
using System.Text;
using FluentMigrator;

namespace Jiggswap.Migrations.Migrations
{
    [Migration(202001272104)]
    public class AddTagsToPuzzles : Migration
    {
        public override void Up()
        {
            Alter.Table("puzzles")
                .AddColumn("tags")
                .AsString(310) // (10 tags * 30 chars) + 10 delimiters
                .Nullable();
        }

        public override void Down()
        {
            Execute.Sql("alter table puzzles drop column tags");
        }
    }
}
