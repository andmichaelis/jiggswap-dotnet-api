using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Migrations.Migrations
{
    [Migration(202006161902)]
    public class AddPerformanceIndexes : Migration
    {
        public override void Up()
        {
            Create.Index("ix_puzzles_userid")
                .OnTable("puzzles")
                .OnColumn("owner_id");

            Create.Index("ix_userprofiles_userid")
                .OnTable("user_profiles")
                .OnColumn("user_id")
                .Unique();
        }

        public override void Down()
        {
            Delete.Index("ix_puzzles_userid");

            Delete.Index("ix_userprofiles_userid");
        }
    }
}