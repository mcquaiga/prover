#region

using Autofac;

#endregion

namespace Prover.Core.Shared.Infrastructure.DependencyManagement
{
    /// <summary>
    ///     Dependency registrar interface
    /// </summary>
    public interface IDependencyRegistrar
    {
        /// <summary>
        ///     Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        void Register<TConfig>(ContainerBuilder builder, ITypeFinder typeFinder, TConfig config) where TConfig : class;

        /// <summary>
        ///     Order of this dependency registrar implementation
        /// </summary>
        int Order { get; }
    }
}