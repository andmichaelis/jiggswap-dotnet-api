using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Migrations.Migrations
{
    [Migration(2020060416278)]
    public class AddUserOAuthData : Migration
    {
        public override void Up()
        {
            Create.Table("user_oauth_data")
                .WithColumn("jiggswap_user_id")
                .AsInt32()
                .NotNullable()
                .ForeignKey("users", "id")

                .WithColumn("service")
                .AsString(20)
                .NotNullable()

                .WithColumn("service_user_id")
                .AsString(100)
                .NotNullable();
        }

        public override void Down()
        {
            Delete.Table("user_oauth_data");
        }
    }
}