using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LiteDB;
using Prover.Application.Services;
using Prover.Shared.Domain;
using Prover.Shared.Storage.Interfaces;

namespace Prover.Storage.LiteDb {
	public class BaseLiteDbAsyncRepository<TId, T> : IAsyncCrudRepository<TId, T> where T : EntityBase<TId> {
		protected readonly ILiteDatabase Context;
		protected readonly ILiteCollection<T> Collection;

		public BaseLiteDbAsyncRepository(ILiteDatabase context) {
			Context = context;
			Collection = Context.GetCollection<T>();
		}

		/// <inheritdoc />
		public Task<bool> DeleteAsync(TId id) {
			var bsonId = new BsonValue(id);
			return Task.FromResult(Collection.Delete(bsonId));
		}

		/// <inheritdoc />
		public Task<T> GetAsync(TId id) {
			var bsonId = new BsonValue(id);
			return Task.FromResult(Collection.FindById(bsonId));
		}


		public Task<T> UpsertAsync(T entity) {
			var success = Collection.Upsert(entity);
			return Task.FromResult(success ? entity : null);
		}
	}
}