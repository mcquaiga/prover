using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Domain;

namespace Domain.EvcVerifications.Verifications
{
    public class AggregateRootWithChildTests<TTest> : AggregateRoot
        where TTest : class
    {
        protected AggregateRootWithChildTests()
        {
        }

        protected AggregateRootWithChildTests(ICollection<TTest> Tests)
        {
            _tests.ToList()
                .AddRange(Tests);
        }

        private ICollection<TTest> _tests = new List<TTest>();

        #region Public Properties

        public ICollection<TTest> Tests
        {
            get => _tests;
            set => _tests = value;
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

        #endregion
    }
}