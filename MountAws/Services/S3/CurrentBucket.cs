using MountAnything;

namespace MountAws.Services.S3;

public class CurrentBucket : TypedString
{
    public string Name => Value;

    public CurrentBucket(string name) : base(name) {}
}