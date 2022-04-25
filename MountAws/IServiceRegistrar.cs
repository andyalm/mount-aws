using Autofac;

namespace MountAws;

public interface IServiceRegistrar
{
    void Register(ContainerBuilder builder);
}