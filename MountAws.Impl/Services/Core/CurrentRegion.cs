using MountAws.Api;

namespace MountAws.Services.Core;

public class CurrentRegion : TypedString
{
    public CurrentRegion(string value) : base(value)
    {
    }
}