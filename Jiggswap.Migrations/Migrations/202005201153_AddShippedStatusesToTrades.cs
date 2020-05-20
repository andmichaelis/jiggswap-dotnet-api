using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Migrations.Migrations
{
    [Migration(202005201153)]
    public class AddPuzzleStatusesToTrades : Migration
    {
        public override void Up()
        {
            Create.Column("requested_puzzle_status")
                .OnTable("trades")
                .AsString(10)
                .Nullable();

            Create.Column("requested_puzzle_shipped_via")
                .OnTable("trades")
                .AsString(50)
                .Nullable();

            Create.Column("requested_puzzle_shipped_trackingno")
                .OnTable("trades")
                .AsString(50)
                .Nullable();

            Create.Column("initiator_puzzle_status")
                .OnTable("trades")
                .AsString(10)
                .Nullable();

            Create.Column("initiator_puzzle_shipped_via")
                .OnTable("trades")
                .AsString(50)
                .Nullable();

            Create.Column("initiator_puzzle_shipped_trackingno")
                .OnTable("trades")
                .AsString(50)
                .Nullable();
        }

        public override void Down()
        {
            Delete.Column("requested_puzzle_status").FromTable("trades");
            Delete.Column("requested_puzzle_shipped_via").FromTable("trades");
            Delete.Column("requested_puzzle_shipped_trackingno").FromTable("trades");

            Delete.Column("initiator_puzzle_status").FromTable("trades");
            Delete.Column("initiator_puzzle_shipped_via").FromTable("trades");
            Delete.Column("initiator_puzzle_shipped_trackingno").FromTable("trades");
        }
    }
}