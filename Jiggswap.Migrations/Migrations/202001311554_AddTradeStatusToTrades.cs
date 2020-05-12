using System;
using System.Collections.Generic;
using System.Text;
using FluentMigrator;

namespace Jiggswap.Migrations.Migrations
{
    [Migration(202001311554)]
    public class AddTradeStatusToTrades : Migration
    {
        public override void Up()
        {
            Alter.Table("trades")
                .AddColumn("status")
                .AsString(15)
                .NotNullable()
                .WithDefaultValue("proposed");
        }

        public override void Down()
        {
            Execute.Sql("alter table trades drop column status");
        }
    }
}
