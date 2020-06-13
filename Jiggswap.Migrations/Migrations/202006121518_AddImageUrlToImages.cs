using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Migrations.Migrations
{
    [Migration(202006121518)]
    public class AddImageUrlToImages : Migration
    {
        public override void Up()
        {
            Alter.Table("images")
                .AddColumn("s3_url")
                .AsString()
                .Nullable()
                .AddColumn("s3_filename")
                .AsString()
                .Nullable()

                .AlterColumn("image_data")
                .AsBinary()
                .Nullable();
        }

        public override void Down()
        {
            Delete.Column("s3_url")
            .FromTable("images");

            Delete.Column("s3_filename")
            .FromTable("images");
        }
    }
}