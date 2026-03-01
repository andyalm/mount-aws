using MountAnything;

namespace MountAws.Services.SecretsManager;

public class SecretPath : TypedItemPath
{
    public SecretPath(ItemPath path) : base(path) { }
}
