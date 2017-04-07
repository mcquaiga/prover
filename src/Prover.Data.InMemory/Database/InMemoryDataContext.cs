using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Data.InMemory.Repositories;
using Prover.Shared.Storage;

namespace Prover.Data.InMemory.Database
{
    internal class InMemoryDataContextFactory : IDataContextFactory<InMemoryDataContext>
    {
        public InMemoryDataContext Create()
        {
            return new InMemoryDataContext();
        }
    }

    internal class InMemoryDataContext
    {
        public List<TestRunDao> TestRuns { get; }

        public InMemoryDataContext()
        {
            TestRuns = new List<TestRunDao>();
        }

        public void AddEntity<T>(T databaseEntity)
        {
            if (databaseEntity is TestRunDao)
            {
                TestRuns.Add(databaseEntity as TestRunDao);
            }    
        }

        public void DeleteEntity<T>(T databaseEntity)
        {
            if (databaseEntity is TestRunDao)
            {
                var testRunDao = databaseEntity as TestRunDao;
                var toBeDeleted = from t in TestRuns where 
            }
        }
    }
}
