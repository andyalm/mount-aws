using MountAnything;

namespace MountAws.Services.Lambda;

public class CurrentFunction : TypedString
{
    public CurrentFunction(string name) : base(name) { }
    public string Name => Value;
}
