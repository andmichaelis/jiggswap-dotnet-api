using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Migrations.Migrations
{
    [Migration(202006181738)]
    public class RenameImageS3UrlColumns : Migration
    {
        public override void Up()
        {
            Rename.Column("s3_url").OnTable("images").To("image_url");
        }

        public override void Down()
        {
            Rename.Column("image_url").OnTable("images").To("s3_url");
        }
    }
}