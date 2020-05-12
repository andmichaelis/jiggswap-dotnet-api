using System;
using System.Collections.Generic;
using System.Text;
using FluentMigrator;

namespace Jiggswap.Migrations.Migrations
{
    [Migration(202002121236)]
    public class AddPasswordResetTokens : Migration
    {
        public override void Up()
        {
            Create.Table("password_reset_tokens")
                .WithColumn("user_id")
                .AsInt32()
                .ForeignKey("users", "id")

                .WithColumn("token")
                .AsString()
                .NotNullable()

                .WithColumn("expiration")
                .AsDateTime()
                .NotNullable()

                .WithColumn("status")
                .AsInt16();
        }

        public override void Down()
        {
            Execute.Sql("drop table password_reset_tokens");
        }
    }
}