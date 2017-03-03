using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Prover.Core.Domain.Models.QaTestRuns;
using Prover.Core.Models.Instruments;
using Prover.Modules.Certificates.Models;

namespace Prover.Web.API.Storage
{
    //public class ProverNextContext : DbContext
    //{
    //    public ProverNextContext()
    //    {
    //        this.LogToConsole();
    //    }

    //    public DbSet<QaTestRunDal> QaTestRuns { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    {
    //        //base.OnConfiguring(optionsBuilder);
    //        optionsBuilder.UseSqlServer(@"Server=.\sqlexpress;Database=prover_api_vNext;Trusted_Connection=True;");
    //    }

        
    //}
}
