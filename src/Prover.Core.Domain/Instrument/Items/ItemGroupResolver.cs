using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMapper;
using Prover.Shared.DTO.TestRuns;
using Prover.Shared.Extensions;

namespace Prover.Domain.Instrument.Items
{
    public class ItemGroupResolver<TSource, TDestination, TDestinationMember> : IValueResolver<TSource, TDestination, TDestinationMember>
        where TSource : TestDtoBase
        where TDestinationMember : class
    {
        private readonly Expression<Func<TSource, Dictionary<string, string>>> _propertyExpression;

        public ItemGroupResolver()
        {
            _propertyExpression = source => source.ItemData;
        }

        public ItemGroupResolver(Expression<Func<TSource, Dictionary<string, string>>> propertyExpression)
        {
            _propertyExpression = propertyExpression;
        }

        public TDestinationMember Resolve(TSource source, TDestination destination, TDestinationMember destMember,
            ResolutionContext context)
        {
            var type = source.GetType();
            if (type == null) throw new NotSupportedException($"Item group type {source} not supported.");

            return Activator.CreateInstance(type, source.GetValue(_propertyExpression)) as TDestinationMember;
        }
    }
}