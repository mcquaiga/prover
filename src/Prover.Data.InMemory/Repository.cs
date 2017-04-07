using System;
using System.Collections.Generic;
using Prover.Data.InMemory.Database;
using Prover.Shared.Domain;
using Prover.Shared.Storage;

namespace Prover.Data.InMemory
{
	public abstract class Repository<TDomain, TId, TDatabase> : IRepository<TDomain, TId>
        where TDomain : AggregateRoot<TId>
	{
	    public InMemoryDataContextFactory ContextFactory { get; }

	    protected Repository(InMemoryDataContextFactory contextFactory)
	    {
	        ContextFactory = contextFactory;
	    }

	    public void Update(TDomain aggregate)
	    {
	        var databaseObject = ConvertToDatabaseType(aggregate);
			ContextFactory.Create().AddEntity(databaseObject);
		}

		public void Insert(TDomain aggregate)
		{
			
		}

		public void Delete(TDomain aggregate)
		{
			
		}

		public abstract TDomain FindBy(TId id);

        public IEnumerable<TDomain> FindAll()
        {
            throw new NotImplementedException();
        }

        public abstract TDatabase ConvertToDatabaseType(TDomain domainType);

		private TDatabase RetrieveDatabaseTypeFrom(IAggregateRoot aggregateRoot)
		{
			TDomain domainType = (TDomain)aggregateRoot;
			TDatabase databaseType = ConvertToDatabaseType(domainType);
			return databaseType;
		}

	    
	}
}
