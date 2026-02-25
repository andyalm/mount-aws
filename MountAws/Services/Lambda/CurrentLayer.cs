using MountAnything;

namespace MountAws.Services.Lambda;

public class CurrentLayer : TypedString
{
    public CurrentLayer(string name) : base(name) { }
    public string Name => Value;
}
