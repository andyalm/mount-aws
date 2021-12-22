using Autofac;

namespace MountAws.Api;

public interface IApiServiceRegistrar
{
    void Register(ContainerBuilder builder);
}