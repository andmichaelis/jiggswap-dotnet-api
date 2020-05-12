using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Data;
using Npgsql;

namespace Jiggswap.Application.Common
{
    public interface IJiggswapDb
    {
        IDbConnection GetConnection();
    }

    public class JiggswapDb : IJiggswapDb
    {
        private readonly string _connectionString;

        public JiggswapDb(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("JiggswapDb");
        }

        public IDbConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
