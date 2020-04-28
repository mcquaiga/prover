using Prover.Application.Models.EvcVerifications.Verifications;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prover.Application.Models.EvcVerifications
{
    public class VerificationTestPoint : VerificationEntity
    {
        private ICollection<VerificationEntity> _tests = new List<VerificationEntity>();

        protected VerificationTestPoint()
        {
        }

        protected internal VerificationTestPoint(int testNumber) => TestNumber = testNumber;

        public VerificationTestPoint(int testNumber, IEnumerable<VerificationEntity> tests, decimal? appliedInput = null)
            : this(testNumber)
        {
            AddTests(tests);
        }

        public int TestNumber { get; set; }

        public override bool Verified => Tests.All(t => t.Verified);

        public ICollection<VerificationEntity> Tests
        {
            get => _tests.ToList();
            private set => _tests = value;
        }

        public void AddTests(VerificationEntity test)
        {
            if (test == null)
                return;

            if (!_tests.Contains(test))
                _tests.Add(test);
        }

        public void AddTests(IEnumerable<VerificationEntity> verificationTests)
        {
            foreach (var vt in verificationTests)
                AddTests(vt);
        }


        public T GetTest<T>() where T : class
        {
            return _tests.FirstOrDefault(t => t.GetType() == typeof(T)) as T;
        }

        public T GetTest<T>(Func<T, bool> filter) where T : class
        {
            var ts = _tests
                .OfType<T>()
                .ToList();

            return ts
                .Where(filter)
                .FirstOrDefault();
        }

        public IEnumerable<T> GetTests<T>() where T : IVerification
        {
            return _tests
                .Where(t => t.GetType() == typeof(T)) as IEnumerable<T>;
        }
    }
}