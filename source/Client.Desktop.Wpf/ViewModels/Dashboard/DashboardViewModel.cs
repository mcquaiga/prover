using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Prover.Domain.EvcVerifications;
using Prover.Shared.Interfaces;
using ReactiveUI;

namespace Client.Desktop.Wpf.ViewModels.Dashboard
{
    public class DashboardViewModel : ReactiveObject
    {
        public DashboardViewModel()
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
