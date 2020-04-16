using System;
using System.Collections.Generic;
using System.Linq;
using Prover.Shared.Domain;

namespace Prover.Application.Models.EvcVerifications.Verifications
{
    public class VerificationTestMixins<TTest> where TTest : class
    {
        private AggregateRootWithChildTests<TTest> _aggregateRootWithChildTests;

        public VerificationTestMixins(AggregateRootWithChildTests<TTest> aggregateRootWithChildTests)
        {
            _aggregateRootWithChildTests = aggregateRootWithChildTests;
        }

        public T GetTest<T>() where T : class
        {
            return _aggregateRootWithChildTests.Tests.FirstOrDefault(t => t.GetType() == typeof(T)) as T;
        }

        public T GetTest<T>(Func<T, bool> filter) where T : class
        {
            var ts = _aggregateRootWithChildTests.Tests
                                                 .OfType<T>()
                                                 .ToList();

            return ts
                   .Where(filter)
                   .FirstOrDefault();
        }

        public IEnumerable<T> GetTests<T>() where T : IVerification
        {
            return _aggregateRootWithChildTests.Tests.OfType<T>();

        }
    }

    public class AggregateRootWithChildTests<TTest> : AggregateRoot
        where TTest : class
    {
        protected AggregateRootWithChildTests()
        {
            _verificationTestMixins = new VerificationTestMixins<TTest>(this);
        }

        //protected AggregateRootWithChildTests(ICollection<TTest> Tests)
        //{
        //    _tests.ToList()
        //        .AddRange(Tests);
        //}

        private ICollection<TTest> _tests = new List<TTest>();
        private readonly VerificationTestMixins<TTest> _verificationTestMixins;

        #region Public Properties

        public ICollection<TTest> Tests
        {
            get => _tests;
            set => _tests = value;
        }

        public VerificationTestMixins<TTest> VerificationTestMixins
        {
            get { return _verificationTestMixins; }
        }

        #endregion

        #region Public Methods

        public void AddTest(TTest test)
        {
            if (!_tests.Contains(test))
                _tests.Add(test);
        }

        public void AddTests(IEnumerable<TTest> test)
        {
            test.ToList().ForEach( _tests.Add);
        }

        #endregion
    }
}