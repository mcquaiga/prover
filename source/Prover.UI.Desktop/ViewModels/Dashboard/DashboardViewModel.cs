using System;
using System.Collections.Generic;
using System.Linq;
using Prover.Application.Models.EvcVerifications;
using Prover.Shared.Interfaces;
using Prover.Shared.Storage.Interfaces;
using ReactiveUI;

namespace Prover.UI.Desktop.ViewModels.Dashboard
{
    public class DashboardingViewModel : ReactiveObject
    {
        public DashboardingViewModel()
        {

        }
    }

    public class DashboardService
    {
        private readonly IAsyncRepository<EvcVerificationTest> _verificationRepository;

        public DashboardService(IAsyncRepository<EvcVerificationTest> verificationRepository)
        {
            _verificationRepository = verificationRepository;
        }

        public VerificationStats GetVerificationStats(DateTime? fromDate = null, DateTime? toDateTime = null)
        {
            var tests = _verificationRepository.Query(test => test.TestDateTime.Between(fromDate.Value, toDateTime.Value));

            return new VerificationStats(tests.ToList());
        }
    }

    public class VerificationStats
    {
        private readonly IEnumerable<EvcVerificationTest> _verificationTests;

        public VerificationStats(ICollection<EvcVerificationTest> verificationTests)
        {
            TotalCount = verificationTests.Count;
            PassCount = verificationTests.Count(t => t.Verified);
            FailCount = verificationTests.Count(t => !t.Verified);
            _verificationTests = verificationTests;
        }

        public VerificationStats(IObservable<EvcVerificationTest> verificationTests)
        {

        }

        public int TotalCount { get; set; }
        public int PassCount { get; set; }
        public int FailCount { get; set; }
    }
}
