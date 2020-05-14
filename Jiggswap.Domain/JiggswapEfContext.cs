using Jiggswap.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Domain
{
    public class JiggswapEfContext : DbContext
    {
        public DbSet<JiggswapUser> Users { get; set; }

        public DbSet<Puzzle> Puzzles { get; set; }

        public JiggswapEfContext(DbContextOptions<JiggswapEfContext> opts) : base(opts)
        {
        }
    }
}