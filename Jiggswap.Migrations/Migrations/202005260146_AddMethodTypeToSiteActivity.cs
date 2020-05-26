using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Migrations.Migrations
{
    [Migration(202005260146)]
    public class AddMethodTypeToSiteActivity : Migration
    {
        public override void Up()
        {
            Create.Column("http_method")
                .OnTable("site_activity")
                .AsString(8)
                .Nullable();
        }

        public override void Down()
        {
            Delete.Column("http_method")
                .FromTable("site_activity");
        }
    }
}