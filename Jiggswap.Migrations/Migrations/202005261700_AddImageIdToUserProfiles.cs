using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Migrations.Migrations
{
    [Migration(202005261700)]
    public class AddImageIdToUserProfiles : Migration
    {
        public override void Up()
        {
            Alter.Table("user_profiles")
                .AddColumn("image_id")
                .AsInt32()
                .ForeignKey("images", "id")
                .Nullable();
        }

        public override void Down()
        {
            Delete.Column("image_id")
                .FromTable("user_profiles");
        }
    }
}