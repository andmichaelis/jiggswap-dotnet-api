using System;
using System.Collections.Generic;
using System.Text;
using FluentMigrator;

namespace Jiggswap.Migrations.Migrations
{
    [Migration(202001231656)]
    public class AddPuzzleTable : Migration
    {
        public override void Up()
        {
            Create.Table("puzzles")
                .WithColumn("id")
                .AsInt32()
                .PrimaryKey()
                .Identity()

                .WithColumn("public_id")
                .AsGuid()
                .Unique()
                .WithDefault(SystemMethods.NewGuid)

                .WithColumn("created_at")
                .AsDateTime()
                .WithDefault(SystemMethods.CurrentDateTime)

                .WithColumn("updated_at")
                .AsDateTime()
                .WithDefault(SystemMethods.CurrentDateTime)

                .WithColumn("owner_id")
                .AsInt32()
                .ForeignKey("users", "id")

                .WithColumn("title")
                .AsString(200)
                .NotNullable()

                .WithColumn("num_pieces")
                .AsInt16()
                .NotNullable()

                .WithColumn("num_pieces_missing")
                .AsInt16()
                .NotNullable()
                .WithDefaultValue(0)

                .WithColumn("additional_notes")
                .AsString()
                .Nullable()
                .WithDefaultValue(string.Empty);
        }

        public override void Down()
        {
            Delete.Table("puzzles");
        }
    }
}
