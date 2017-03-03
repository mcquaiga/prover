using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Prover.Core.DAL.DataAccess.QaTestRuns;
using Prover.Core.Domain.Models.QaTestRuns;

namespace Prover.Core.DAL
{
    public class Startup
    {
        public Startup()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<QaTestRunDto, QaTestRunDal>());
        }
    }
}
