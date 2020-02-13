using System;
using System.Collections.Generic;
using System.Linq;
using Domain.EvcVerifications.Verifications;

namespace Domain.EvcVerifications
{
    public class VerificationTestPoint : VerificationEntity, IVerification
    {
        protected VerificationTestPoint() {}

        protected internal VerificationTestPoint(int testNumber) : base($"Level #{testNumber}")
        {
            TestNumber = testNumber;
        }

        public VerificationTestPoint(int testNumber, IEnumerable<VerificationEntity> tests, decimal? appliedInput = null) 
            : this(testNumber)
        {
            AddTests(tests);
            AppliedInput = appliedInput;
        }

        #region Public Properties
        public decimal? AppliedInput { get; set; }
        public int TestNumber { get; set; }
        #endregion

        //public string Description { get; }
        //public bool Verified => Tests.All(t => t.Verified);


        protected ICollection<VerificationEntity> TestsCollection => _tests;
        private ICollection<VerificationEntity> _tests = new List<VerificationEntity>();


        public T GetTest<T>() where T : class
        {
            return TestsCollection.FirstOrDefault(t => t.GetType() == typeof(T)) as T;
        }

        public T GetTest<T>(Func<T, bool> filter) where T : class
        {
            var ts = TestsCollection
                .OfType<T>()
                .ToList();

            return (T) ts
                .Where(filter)
                .FirstOrDefault();
        }

        public IEnumerable<T> GetTests<T>() where T : IVerification
        {
            return TestsCollection
                .Where(t => t.GetType() == typeof(T)) as IEnumerable<T>;
        }

        public ICollection<VerificationEntity> Tests
        {
            get => _tests;
            set => _tests = value;
        }

        public void AddTest(VerificationEntity test)
        {
            if (!TestsCollection.Contains(test))
                TestsCollection.Add(test);
        }

        public void AddTests(IEnumerable<VerificationEntity> test)
        {
            test.ToList().ForEach(AddTest);
        }
    }
}