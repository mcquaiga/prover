using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData.Annotations;
using DynamicData.Kernel;

namespace Prover.Application.Extensions
{
    public static class ReactiveEx
    {
        public static IObservable<Unit> AsCompletion<T>(this IObservable<T> observable)
        {
            return Observable.Create<Unit>(observer =>
            {
                void OnCompleted()
                {
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                }

                return observable.Subscribe(_ => { }, observer.OnError, OnCompleted);
            });
        }

        public static IObservable<bool> HasChanged<T>([NotNull] this IObservable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source.Scan((InitialAndCurrent<T>) null,
                    (acc, current) =>
                    {
                        return acc == null
                            ? new InitialAndCurrent<T>(current)
                            : new InitialAndCurrent<T>(acc.Initial, current);
                    })
                .Select(initalAndCurrent =>
                {
                    if (!initalAndCurrent.Latest.HasValue) return false;
                    return !initalAndCurrent.Initial.Equals(initalAndCurrent.Latest.Value);
                });
        }

        public static void Once(this ISubject<Unit> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            source.OnNext(Unit.Default);
        }


        public static IObservable<CurrentAndPrevious<TSource>> PairWithPrevious<TSource>(
            this IObservable<TSource> source)
        {
            return source.Scan(Tuple.Create(default(TSource), default(TSource)),
                    (acc, current) => Tuple.Create(acc.Item2, current))
                .Select(pair => new CurrentAndPrevious<TSource>(pair.Item1, pair.Item2));
        }

        public static IObservable<TSource> Previous<TSource>(this IObservable<TSource> source)
        {
            return source.PairWithPrevious().Select(pair => pair.Previous);
        }

        public static IDisposable SetAsComplete<T>(this ISubject<T> source) => Disposable.Create(source.OnCompleted);

        public static IObservable<Unit> StartWithUnit(this IObservable<Unit> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.StartWith(Unit.Default);
        }

        public static IObservable<Unit> ToUnit<T>(this IObservable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.Select(_ => Unit.Default);
        }

        /// <summary>
        ///     from here http://haacked.com/archive/2012/10/08/writing-a-continueafter-method-for-rx.aspx/
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="observable"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IObservable<TRet> WithContinuation<T, TRet>(
            this IObservable<T> observable, Func<IObservable<TRet>> selector)
        {
            return observable.AsCompletion().SelectMany(_ => selector());
        }

        #region Nested type: CurrentAndPrevious

        public class CurrentAndPrevious<T>
        {
            public CurrentAndPrevious(T currrent, T previous)
            {
                Currrent = currrent;
                Previous = previous;
            }

            public T Currrent { get; }
            public T Previous { get; }
        }

        #endregion

        #region Nested type: InitialAndCurrent

        private class InitialAndCurrent<T>
        {
            public InitialAndCurrent(T initial, Optional<T> current)
            {
                Latest = current;
                Initial = initial;
            }

            public InitialAndCurrent(T initial)
            {
                Latest = Optional<T>.None;
                Initial = initial;
            }

            public Optional<T> Latest { get; }
            public T Initial { get; }
        }

        #endregion
    }
}