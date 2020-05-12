using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Extensions.Logging;
using Prover.Application;
using ReactiveUI;

namespace Prover.UI.Desktop.Extensions
{
	public static class WpfExtensions
	{
		//public static IDisposable CleanupChildren<T>(this IViewFor parent)
		//    where T : DependencyObject
		//{
		//    return CleanupChildren<T>((ContentControl) parent);
		//}
		private static readonly ILogger _logger = ProverLogging.CreateLogger(typeof(WpfExtensions));

		public static IDisposable CleanupChildren<T>(this DependencyObject parentView, ILogger logger = null)
			where T : DependencyObject
		{
			logger ??= _logger;

			if (parentView != null)
				return Disposable.Create(() =>
				{
					var children = parentView.FindAllChilds<T>();
					foreach (var child in children.Reverse())
					{
						// ReSharper disable once SuspiciousTypeConversion.Global
						if (child is IDisposable disposable)
						{
							logger.LogTrace($"Disposing Child - Name:  {child.AsOrDefault<ViewModelViewHost>()?.Name} - ViewModel: {child.AsOrDefault<ViewModelViewHost>()?.ViewModel}");
							disposable.Dispose();
						}
					}

					children.ForEach(c => c = null);
					BindingOperations.ClearAllBindings(parentView);
					GC.SuppressFinalize(parentView);
				});

			logger.LogTrace($"{parentView} is not a {nameof(DependencyObject)}");
			return Disposable.Empty;
		}

		public static IDisposable CleanUpDefaults(this DependencyObject view, ILogger logger = null)
		{
			logger ??= _logger;

			return new CompositeDisposable(
				LogDisposable($"Disposed IViewFor - {view.GetType().Name}", logger),
				view.CleanupChildren<ViewModelViewHost>()
			);
		}

		public static ICollection<T> FindAllChilds<T>(this DependencyObject parent, string childName = "")
			where T : DependencyObject
		{
			var children = new List<T>();
			// Confirm parent and childName are valid. 
			if (parent == null)
				return null;

			T foundChild = null;

			var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
			for (var i = 0; i < childrenCount; i++)
			{
				foundChild = null;
				var child = VisualTreeHelper.GetChild(parent, i);
				// If the child is not of the request child type child
				var childType = child as T;
				if (childType == null)
				{
					// recursively drill down the tree
					children.AddRange(
						FindAllChilds<T>(child, childName));
					// If the child is found, break so we do not overwrite the found child. 
				}
				else if (!string.IsNullOrEmpty(childName))
				{
					var frameworkElement = child as FrameworkElement;
					// If the child's name is set for search
					if (frameworkElement != null && frameworkElement.Name == childName)
						// if the child's name is of the request name
						foundChild = (T)child;
				}
				else
				{
					// child element found.
					foundChild = (T)child;
				}

				if (foundChild != null)
					children.Add(foundChild);
			}

			return children;
		}

		/// <summary>
		///     Finds a Child of a given item in the visual tree.
		/// </summary>
		/// <param name="parent">A direct parent of the queried item.</param>
		/// <typeparam name="T">The type of the queried item.</typeparam>
		/// <param name="childName">x:Name or Name of child. </param>
		/// <returns>
		///     The first parent item that matches the submitted type parameter.
		///     If not matching item can be found,
		///     a null parent is being returned.
		/// </returns>
		public static T FindChild<T>(this DependencyObject parent, string childName = "")
			where T : DependencyObject
		{
			// Confirm parent and childName are valid. 
			if (parent == null)
				return null;

			T foundChild = null;

			var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
			for (var i = 0; i < childrenCount; i++)
			{
				var child = VisualTreeHelper.GetChild(parent, i);
				// If the child is not of the request child type child
				var childType = child as T;
				if (childType == null)
				{
					// recursively drill down the tree
					foundChild = FindChild<T>(child, childName);

					// If the child is found, break so we do not overwrite the found child. 
					if (foundChild != null)
						break;
				}
				else if (!string.IsNullOrEmpty(childName))
				{
					var frameworkElement = child as FrameworkElement;
					// If the child's name is set for search
					if (frameworkElement != null && frameworkElement.Name == childName)
					{
						// if the child's name is of the request name
						foundChild = (T)child;
						break;
					}
				}
				else
				{
					// child element found.
					foundChild = (T)child;
					break;
				}
			}

			return foundChild;
		}

		public static IDisposable LogDisposable(string message, ILogger logger = null)
		{
			logger ??= _logger;
			return Disposable.Create(() => logger.LogTrace((message)));
		}

	}



}