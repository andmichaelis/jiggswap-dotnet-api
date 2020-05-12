using System;
using System.Collections.Generic;
using System.Text;
using FluentMigrator;

namespace Jiggswap.Migrations.Migrations
{
    [Migration(202002061510)]
    public class AddUserProfiles : Migration
    {
        public override void Up()
        {
            Create.Table("user_profiles")
                .WithColumn("id")
                .AsInt32()
                .Identity()

                .WithColumn("user_id")
                .AsInt32()
                .ForeignKey("users", "id")

                .WithColumn("firstname")
                .AsString(50)
                .Nullable()

                .WithColumn("lastname")
                .AsString(50)
                .Nullable();
        }

        public override void Down()
        {
            Execute.Sql("drop table user_profiles");
        }
    }
}
