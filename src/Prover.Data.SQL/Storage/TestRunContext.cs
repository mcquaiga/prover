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
    public class TestRunSqlContext : SqlDataContext
    {
        public virtual DbSet<TestRunDao> TestRuns { get; set; }
    }
}
