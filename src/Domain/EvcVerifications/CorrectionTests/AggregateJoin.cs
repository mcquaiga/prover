using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Shared.Domain;

namespace Domain.EvcVerifications.CorrectionTests
{
    public class AggregateRootWithChildTests<TTest> : AggregateRoot
        where TTest : class
    {
        protected AggregateRootWithChildTests() { }

        protected AggregateRootWithChildTests(ICollection<TTest> testsCollection)
        {
            TestsCollection.ToList()
                .AddRange(testsCollection);
        }

        protected readonly ICollection<TTest> TestsCollection = new HashSet<TTest>();


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

        public IEnumerable<T> GetTests<T>() where T : IVerificationTest
        {
            return TestsCollection
                .Where(t => t.GetType() == typeof(T)) as IEnumerable<T>;
        }

        public IReadOnlyCollection<TTest> Tests => TestsCollection.ToList().AsReadOnly();

        public void AddTest(TTest test)
        {
            if (!TestsCollection.Contains(test))
                TestsCollection.Add(test);
        }

        public void AddTest(IEnumerable<TTest> test)
        {
            test.ToList().ForEach(AddTest);
        }
    }
}