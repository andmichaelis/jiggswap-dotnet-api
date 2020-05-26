using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Migrations.Migrations
{
    [Migration(202005260026)]
    public class AddSiteActivity : Migration
    {
        public override void Up()
        {
            Create.Table("site_activity")
                .WithColumn("id")
                .AsInt32()
                .Identity()

                .WithColumn("user_id")
                .AsInt32()
                .Nullable()

                .WithColumn("timestamp")
                .AsDateTime()
                .WithDefault(SystemMethods.CurrentDateTime)

                .WithColumn("path")
                .AsString(50)
                .Nullable()

                .WithColumn("ip_address")
                .AsString(50)
                .Nullable();
        }

        public override void Down()
        {
            Delete.Table("site_activity");
        }
    }
}