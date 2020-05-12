using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Migrations.Migrations
{
    [Migration(202001271749)]
    public class AddImages : Migration
    {
        public override void Up()
        {
            Create.Table("images")
                .WithColumn("id")
                .AsInt32()
                .PrimaryKey()
                .Identity()

                .WithColumn("image_data")
                .AsBinary(2000000) // 2 mb?
                .NotNullable();

            Alter.Table("puzzles")
                .AddColumn("image_id")
                .AsInt32()
                .ForeignKey("images", "id")
                .Nullable();
        }

        public override void Down()
        {
            Execute.Sql("alter table puzzles drop column image_id");
            Execute.Sql("drop table images");
        }
    }
}
