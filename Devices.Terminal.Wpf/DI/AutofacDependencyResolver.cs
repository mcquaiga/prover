using Autofac;
using Autofac.Core;
using Splat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AutofacDependencyResolver : IMutableDependencyResolver
{
    #region Constructors

    public AutofacDependencyResolver(IContainer container)
    {
        _container = container;
    }

    #endregion

    #region Methods

    public void Dispose()
    {
        _container.Dispose();
    }

    public object GetService(Type serviceType, string contract = null)
    {
        try
        {
            return string.IsNullOrEmpty(contract)
                ? _container.Resolve(serviceType)
                : _container.ResolveNamed(contract, serviceType);
        }
        catch (DependencyResolutionException)
        {
            return null;
        }
    }

    public IEnumerable<object> GetServices(Type serviceType, string contract = null)
    {
        try
        {
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(serviceType);
            object instance = string.IsNullOrEmpty(contract)
                ? _container.Resolve(enumerableType)
                : _container.ResolveNamed(contract, enumerableType);
            return ((IEnumerable)instance).Cast<object>();
        }
        catch (DependencyResolutionException)
        {
            return null;
        }
    }

    public void Register(Func<object> factory, Type serviceType, string contract = null)
    {
        var builder = new ContainerBuilder();
        if (string.IsNullOrEmpty(contract))
        {
            builder.Register(x => factory()).As(serviceType).AsImplementedInterfaces();
        }
        else
        {
            builder.Register(x => factory()).Named(contract, serviceType).AsImplementedInterfaces();
        }

        builder.Update(_container);
    }

    public IDisposable ServiceRegistrationCallback(Type serviceType, string contract, Action<IDisposable> callback)
    {
        // this method is not used by RxUI
        throw new NotImplementedException();
    }

    public void UnregisterAll(Type serviceType, string contract = null)
    {
        var builder = new ContainerBuilder();
        _container = builder.Build();
    }

    public void UnregisterCurrent(Type serviceType, string contract = null)
    {
    }

    #endregion

    #region Fields

    private IContainer _container;

    #endregion
}