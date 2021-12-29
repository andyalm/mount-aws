using Autofac;

namespace MountAws.Api;

public interface IServiceRegistrar
{
    void Register(ContainerBuilder builder);
}