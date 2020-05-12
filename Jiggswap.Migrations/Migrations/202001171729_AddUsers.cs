using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace JiggswapMigrations.Migrations
{
    [Migration(202001171729)]
    public class AddUsers : Migration
    {
        public override void Up()
        {
            Create.Table("users")
                .WithColumn("id")
                .AsInt32()
                .PrimaryKey()
                .NotNullable()
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

                .WithColumn("username")
                .AsString(50)
                .Unique()
                .NotNullable()

                .WithColumn("email")
                .AsString(50)
                .Unique()
                .NotNullable()

                .WithColumn("password_hash")
                .AsString()
                .NotNullable();
        }

        public override void Down()
        {
            Delete.Table("users");
        }
    }
}
