using System;
using System.Linq.Expressions;
using AutoMapper;
using Newtonsoft.Json;
using Prover.Shared.Extensions;

namespace Prover.Storage.EF.Mappers.Resolvers
{
    public class DeserializeResolver<TSource, TSourceProperty, TDestination, TDestinationMember> :
        IValueResolver<TSource, TDestination, TDestinationMember>
        where TSource : class
    {
        private readonly Expression<Func<TSource, TSourceProperty>> _propertyExpression;

        public DeserializeResolver(Expression<Func<TSource, TSourceProperty>> propertyExpression)
        {
            _propertyExpression = propertyExpression;
        }

        public TDestinationMember Resolve(TSource source, TDestination destination, TDestinationMember destMember,
            ResolutionContext context)
        {
            var sourceValue = source.GetValue(_propertyExpression);

            if (sourceValue is string == false)
                return default(TDestinationMember);

            return JsonConvert.DeserializeObject<TDestinationMember>(sourceValue as string);
        }
    }
}