using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;

namespace Application.Extensions
{
    public static class ObservableEx
    {
        public static IObservable<PropertyChangedEventArgs> FromPropertyChanged<T, TResult>(this T target, Expression<Func<T, TResult>> property)
            where T : INotifyPropertyChanged
        {
            Contract.Requires(target != null);
            Contract.Requires(property != null);

            var body = property.Body as MemberExpression;

            if (body == null)
                throw new ArgumentException("The specified expression does not reference a property.", "property");

            var propertyInfo = body.Member as PropertyInfo;

            if (propertyInfo == null)
                throw new ArgumentException("The specified expression does not reference a property.", "property");

            string propertyName = propertyInfo.Name;

            var propertyDescriptor = (from p in TypeDescriptor.GetProperties(target).Cast<PropertyDescriptor>()
                    where string.Equals(p.Name, propertyName, StringComparison.Ordinal)
                    select p)
                .Single();

            if (!propertyDescriptor.SupportsChangeEvents)
                throw new ArgumentException("The specified property does not support change events.", "property");

            var getter = property.Compile();

            return Observable.FromEventPattern<EventHandler, PropertyChangedEventArgs>(
                h => propertyDescriptor.AddValueChanged(target, h),
                h => propertyDescriptor.RemoveValueChanged(target, h))
                .Select(f => f.EventArgs);

            //select getter(target);
        }

        public static IObservable<R> ToObservable<T, R>( this T target, Expression<Func<T, R>> property ) where T : INotifyPropertyChanged {
            var f = ( property as LambdaExpression ).Body as MemberExpression;

            if( f == null )
                throw new NotSupportedException( "Only use expressions that call a single property" );

            var propertyName = f.Member.Name;
            var getValueFunc = property.Compile( );
            return Observable.Create<R>( o => {
                PropertyChangedEventHandler eventHandler = new PropertyChangedEventHandler( ( s, pce ) => {
                    if( pce.PropertyName == null || pce.PropertyName == propertyName )
                        o.OnNext( getValueFunc( target ) );
                } );
                target.PropertyChanged += eventHandler;
                return ( ) => target.PropertyChanged -= eventHandler;
            } );
        }
    }
    //public static class NotifyPropertyChangeReactiveExtensions
    //{
    //    // Returns the values of property (an Expression) as they change, 
    //    // starting with the current value
    //    public static IObservable<TValue> GetPropertyValues<TSource, TValue>(
    //        this TSource source, Expression<Func<TSource, TValue>> property)
    //        where TSource : INotifyPropertyChanged
    //    {
    //        MemberExpression memberExpression = property.Body as MemberExpression;

    //        if (memberExpression == null)
    //        {
    //            throw new ArgumentException(
    //                "property must directly access a property of the source");
    //        }

    //        string propertyName = memberExpression.Member.Name;

    //        Func<TSource, TValue> accessor = property.Compile();

    //        return source.GetPropertyChangedEvents()
    //            .Where(x => x.EventArgs.PropertyName == propertyName)
    //            .Select(x => accessor(source))
    //            .StartWith(accessor(source));
    //    }

    //    // This is a wrapper around FromEvent(PropertyChanged)
    //    public static IObservable<IEvent<PropertyChangedEventArgs>>
    //        GetPropertyChangedEvents(this INotifyPropertyChanged source)
    //    {
    //        return Observable.FromEvent<PropertyChangedEventHandler,
    //            PropertyChangedEventArgs>(
    //            h => new PropertyChangedEventHandler(h),
    //            h => source.PropertyChanged += h,
    //            h => source.PropertyChanged -= h);
    //    }
    //}
}
