using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace JiggswapMigrations.Migrations
{
    [Migration(0)]
    public class EnableUuidGeneration : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"CREATE EXTENSION IF NOT EXISTS ""uuid-ossp"";");
        }

        public override void Down()
        {
            Execute.Sql(@"DROP EXTENSION IF EXISTS ""uuid-ossp"";");
        }
    }
}
