using MountAnything;

namespace MountAws.Services.Ecs;

public class CurrentService(string serviceName) : TypedString(serviceName)
{
    public string Name => Value;
}