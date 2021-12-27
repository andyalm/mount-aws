using System.Management.Automation;
using MountAnything;

namespace MountAws;

public abstract class AwsItem : Item
{
    protected AwsItem(string parentPath, PSObject underlyingObject) : base(parentPath, underlyingObject)
    {
    }

    public override string TypeName => GetType().FullName!;
}