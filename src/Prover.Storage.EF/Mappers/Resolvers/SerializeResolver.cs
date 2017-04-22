using System;
using System.Linq.Expressions;
using AutoMapper;
using Newtonsoft.Json;
using Prover.Shared.Extensions;

namespace Prover.Storage.EF.Mappers.Resolvers
{
    public class SerializeResolver<TSource, TSourceProperty, TDestination> :
        IValueResolver<TSource, TDestination, string>
        where TSource : class
    {
        private readonly Expression<Func<TSource, TSourceProperty>> _propertyExpression;

        public SerializeResolver(Expression<Func<TSource, TSourceProperty>> propertyExpression)
        {
            _propertyExpression = propertyExpression;
        }

        public string Resolve(TSource source, TDestination destination, string destMember, ResolutionContext context)
        {
            var sourceValue = source.GetValue(_propertyExpression);
            return JsonConvert.SerializeObject(sourceValue);
        }
    }
}