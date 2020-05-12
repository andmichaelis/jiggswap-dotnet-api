using System;
using System.Collections.Generic;
using System.Text;
using FluentMigrator;

namespace Jiggswap.Migrations.Migrations
{
    [Migration(202001291713)]
    public class AddTrades : Migration
    {
        public override void Up()
        {
            Create.Table("trades")
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

                .WithColumn("requested_puzzle_id")
                .AsInt32()
                .ForeignKey("puzzles", "id")

                .WithColumn("requested_user_id")
                .AsInt32()
                .ForeignKey("users", "id")

                .WithColumn("initiator_puzzle_id")
                .AsInt32()
                .ForeignKey("puzzles", "id")

                .WithColumn("initiator_user_id")
                .AsInt32()
                .ForeignKey("users", "id");
        }

        public override void Down()
        {
            Execute.Sql("drop table trades");
        }
    }
}
