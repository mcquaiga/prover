using Autofac;

namespace Prover.GUI.Common.Screens.MainMenu
{
    public interface IModule
    {
        void Configure(ContainerBuilder builder);
    }
}