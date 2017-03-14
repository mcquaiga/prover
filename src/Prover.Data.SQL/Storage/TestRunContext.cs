using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Prover.Data.SQL.Common;
using Prover.Data.SQL.Models;

namespace Prover.Data.SQL.Storage
{
    internal class TestRunSqlContext : SqlDataContext
    {
        public virtual DbSet<TestRunDao> TestRuns { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

        }
    }
}
