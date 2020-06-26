using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Migrations.Migrations
{
    [Migration(202006261600)]
    public class ConvertNumPiecesMissingToText : Migration
    {
        public override void Up()
        {
            Alter
                .Table("puzzles")
                .AlterColumn("num_pieces_missing")
                .AsString(20);
        }

        public override void Down()
        {
        }
    }
}