using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Prover.Domain.Services;

namespace Prover.Domain
{
    public class ProverDomain
    {
        public void Initialize(ContainerBuilder builder)
        {
            //Use SQL data project
            var data = new Prover.Data.SQL.Startup(builder);

            builder.RegisterType<TestRunService>().SingleInstance();
        }
    }
}
