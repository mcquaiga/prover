using System;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Prover.Core.Shared.Data;
using Prover.Core.Shared.Domain;

namespace Prover.Core.Storage
{   
    public class KeyValueStore : ProverStore<KeyValue, string>
    {
        public KeyValueStore(ProverContext dbContext) : base(dbContext)
        {
        }

        public override async Task<KeyValue> Upsert(KeyValue entity)
        {
            var state = Context.Entry(entity).State;
            var existing = GetExistingKeyValue(entity);
            if (existing == null)
            {
                Context.Set<KeyValue>().Add(entity);
                Context.Entry(entity).State = EntityState.Added;
            }
            else
            {
                existing.Value = entity.Value;
                Context.Entry(existing).State = EntityState.Modified;
            }

            await Context.SaveChangesAsync();
            return existing ?? entity;
        }

        private KeyValue GetExistingKeyValue(KeyValue entity)
        {
            var existing = Query(kv => kv.Id.ToLower() == entity.Id.ToLower()).FirstOrDefault();
            return existing;
        }
       
        public override Task<bool> Delete(KeyValue entity)
        {
            var existing = GetExistingKeyValue(entity);
            if (existing == null) throw new ObjectNotFoundException($"Could not find value with key '{entity.Id}' in database.");
            return base.Delete(existing);            
        }
    }
}
