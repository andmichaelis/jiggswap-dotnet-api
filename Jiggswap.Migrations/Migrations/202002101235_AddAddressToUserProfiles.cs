using System;
using System.Collections.Generic;
using System.Text;
using FluentMigrator;

namespace Jiggswap.Migrations.Migrations
{
    [Migration(202002101235)]
    public class AddAddressToUserProfiles : Migration
    {
        public override void Up()
        {
            Alter.Table("user_profiles")
                .AddColumn("streetaddress")
                .AsString()
                .Nullable()

                .AddColumn("city")
                .AsString()
                .Nullable()

                .AddColumn("state")
                .AsString(2)
                .Nullable()

                .AddColumn("zip")
                .AsString(10)
                .Nullable();
        }

        public override void Down()
        {
            Execute.Sql("alter table user_profiles drop column streetaddress");
            Execute.Sql("alter table user_profiles drop column city");
            Execute.Sql("alter table user_profiles drop column state");
            Execute.Sql("alter table user_profiles drop column zip");
        }
    }
}
