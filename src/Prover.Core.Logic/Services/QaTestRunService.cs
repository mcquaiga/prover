using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.DAL.DataAccess.QaTestRuns;
using Prover.Core.Domain.Models.QaTestRuns;

namespace Prover.Core.Logic.Services
{
    public class QaTestRunService
    {
        private readonly QaTestRunRepository _qaTestRunRepository;

        public QaTestRunService(QaTestRunRepository qaTestRunRepository)
        {
            _qaTestRunRepository = qaTestRunRepository;
        }

        public async Task<IEnumerable<QaTestRunDto>> GetAllTestRuns()
        {
            return await _qaTestRunRepository.GetAllAsync(access => access.ExportedDateTime == null);
        }

    }
}
