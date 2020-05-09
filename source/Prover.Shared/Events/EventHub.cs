using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace Prover.Shared.Events
{
	public interface IEventHub<TInput, TOutput>
	{
		/// <summary>
		/// Registers a synchronous event handler.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This overload of <c>RegisterHandler</c> is only useful if the handler can handle the event
		/// immediately. That is, it does not need to wait for a user or some other collaborating component.
		/// </para>
		/// </remarks>
		/// <param name="handler">
		/// The handler.
		/// </param>
		/// <returns>
		/// A disposable which, when disposed, will unregister the handler.
		/// </returns>
		IDisposable Subscribe(Action<EventContext<TInput, TOutput>> handler);

		/// <summary>
		/// Registers a task-based asynchronous event handler.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This overload of <c>RegisterHandler</c> is useful if the handler needs to perform some asynchronous
		/// operation, such as displaying a dialog and waiting for the user's response.
		/// </para>
		/// </remarks>
		/// <param name="handler">
		/// The handler.
		/// </param>
		/// <returns>
		/// A disposable which, when disposed, will unregister the handler.
		/// </returns>
		IDisposable Subscribe(Func<EventContext<TInput, TOutput>, Task> handler);

		/// <summary>
		/// Registers an observable-based asynchronous event handler.
		/// </summary>
		/// <typeparam name="TDontCare">The signal type.</typeparam>
		/// <remarks>
		/// <para>
		/// This overload of <c>RegisterHandler</c> is useful if the handler needs to perform some asynchronous
		/// operation, such as displaying a dialog and waiting for the user's response.
		/// </para>
		/// </remarks>
		/// <param name="handler">
		/// The handler.
		/// </param>
		/// <returns>
		/// A disposable which, when disposed, will unregister the handler.
		/// </returns>
		IDisposable Subscribe<TDontCare>(Func<EventContext<TInput, TOutput>, IObservable<TDontCare>> handler);

		/// <summary>
		/// Handles an event and asynchronously returns the result.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method passes the event through to relevant handlers in reverse order of registration,
		/// ceasing once any handler handles the event. If the event remains unhandled after all
		/// relevant handlers have executed, an <see cref="UnhandledeventException{TInput, TOutput}"/> is thrown.
		/// </para>
		/// </remarks>
		/// <param name="input">
		/// The input for the event.
		/// </param>
		/// <returns>
		/// An observable that ticks when the event completes, or throws an
		/// <see cref="UnhandledeventException{TInput, TOutput}"/> if no handler handles the event.
		/// </returns>
		IObservable<TOutput> Publish(TInput input);
	}

	public sealed class EventHub<TInput, TOutput> : IEventHub<TInput, TOutput>
	{
		private readonly IList<Func<EventContext<TInput, TOutput>, IObservable<Unit>>> _handlers;
		private readonly object _sync;
		private readonly IScheduler _handlerScheduler;

		/// <summary>
		/// Initializes a new instance of the <see cref="EventHub{TInput, TOutput}"/> class.
		/// </summary>
		/// <param name="handlerScheduler">
		/// The scheduler to use when invoking handlers, which defaults to <c>CurrentThreadScheduler.Instance</c> if <see langword="null"/>.
		/// </param>
		public EventHub(IScheduler handlerScheduler = null)
		{
			_handlers = new List<Func<EventContext<TInput, TOutput>, IObservable<Unit>>>();
			_sync = new object();
			_handlerScheduler = handlerScheduler ?? CurrentThreadScheduler.Instance;
		}

		/// <summary>
		/// Registers a synchronous event handler.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This overload of <c>RegisterHandler</c> is only useful if the handler can handle the event
		/// immediately. That is, it does not need to wait for a user or some other collaborating component.
		/// </para>
		/// </remarks>
		/// <param name="handler">
		/// The handler.
		/// </param>
		/// <returns>
		/// A disposable which, when disposed, will unregister the handler.
		/// </returns>
		public IDisposable Subscribe(Action<EventContext<TInput, TOutput>> handler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException(nameof(handler));
			}

			return Subscribe(message =>
			{
				handler(message);
				return Observable.Empty<Unit>();
			});
		}

		/// <summary>
		/// Registers a task-based asynchronous event handler.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This overload of <c>RegisterHandler</c> is useful if the handler needs to perform some asynchronous
		/// operation, such as displaying a dialog and waiting for the user's response.
		/// </para>
		/// </remarks>
		/// <param name="handler">
		/// The handler.
		/// </param>
		/// <returns>
		/// A disposable which, when disposed, will unregister the handler.
		/// </returns>
		public IDisposable Subscribe(Func<EventContext<TInput, TOutput>, Task> handler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException(nameof(handler));
			}

			return Subscribe(message => handler(message).ToObservable());
		}

		/// <summary>
		/// Registers an observable-based asynchronous event handler.
		/// </summary>
		/// <typeparam name="TDontCare">The signal type.</typeparam>
		/// <remarks>
		/// <para>
		/// This overload of <c>RegisterHandler</c> is useful if the handler needs to perform some asynchronous
		/// operation, such as displaying a dialog and waiting for the user's response.
		/// </para>
		/// </remarks>
		/// <param name="handler">
		/// The handler.
		/// </param>
		/// <returns>
		/// A disposable which, when disposed, will unregister the handler.
		/// </returns>
		public IDisposable Subscribe<TDontCare>(Func<EventContext<TInput, TOutput>, IObservable<TDontCare>> handler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException(nameof(handler));
			}

			Func<EventContext<TInput, TOutput>, IObservable<Unit>> unitHandler = context => handler(context).Select(_ => Unit.Default);

			AddHandler(unitHandler);
			return Disposable.Create(() => RemoveHandler(unitHandler));
		}

		/// <summary>
		/// Handles an event and asynchronously returns the result.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method passes the event through to relevant handlers in reverse order of registration,
		/// ceasing once any handler handles the event. If the event remains unhandled after all
		/// relevant handlers have executed, an <see cref="UnhandledeventException{TInput, TOutput}"/> is thrown.
		/// </para>
		/// </remarks>
		/// <param name="input">
		/// The input for the event.
		/// </param>
		/// <returns>
		/// An observable that ticks when the event completes, or throws an
		/// <see cref="UnhandledeventException{TInput, TOutput}"/> if no handler handles the event.
		/// </returns>
		public IObservable<TOutput> Publish(TInput input)
		{
			var contexts = new ConcurrentBag<EventContext<TInput, TOutput>>();
			return GetHandlers()
				   .Reverse()
				   .ToObservable()
				   .ObserveOn(_handlerScheduler)
				   .Select(handler =>
				   {
					   var ctx = new EventContext<TInput, TOutput>(input);
					   contexts.Add(ctx);
					   return Observable.Defer(() => handler(ctx));
				   })
				   .Concat()
				   .TakeUntil(_ => contexts.Any(c => c.IsHandled))
				   .IgnoreElements()
				   .Select(_ => default(TOutput))
				   .Concat(
				   Observable.Defer(
				   () =>
				   {
					   var output = contexts.LastOrDefault(c => c.IsHandled);
					   var result = output != null ? output.GetOutput() : default(TOutput);
					   return Observable.Return(result);
				   }));
			//: Observable.Throw<TOutput>(new UnhandledErrorException("Error"))));
			//: Observable.Throw<TOutput>(new UnhandledeventException<TInput, TOutput>(this, input))));
		}

		/// <summary>
		/// Gets all registered handlers in order of their registration.
		/// </summary>
		/// <returns>
		/// All registered handlers.
		/// </returns>
		private IEnumerable<Func<EventContext<TInput, TOutput>, IObservable<Unit>>> GetHandlers()
		{
			lock (_sync)
			{
				return _handlers.ToArray();
			}
		}

		private void AddHandler(Func<EventContext<TInput, TOutput>, IObservable<Unit>> handler)
		{
			lock (_sync)
			{
				_handlers.Add(handler);
			}
		}

		private void RemoveHandler(Func<EventContext<TInput, TOutput>, IObservable<Unit>> handler)
		{
			lock (_sync)
			{
				_handlers.Remove(handler);
			}
		}
	}
}