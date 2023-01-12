using MountAnything;

namespace MountAws.Services.Ecs;

public class CurrentCluster : TypedString
{
    public CurrentCluster(string name) : base(name) { }
    public string Name => Value;
}