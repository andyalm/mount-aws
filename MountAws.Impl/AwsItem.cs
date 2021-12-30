using System.Management.Automation;
using MountAnything;

namespace MountAws;

public abstract class AwsItem : Item
{
    protected AwsItem(string parentPath, PSObject underlyingObject) : base(parentPath, underlyingObject)
    {
    }

    public override string TypeName => GetType().FullName!;
    
    public override string ItemType => GetType().Name.EndsWith("Item")
        ? GetType().Name.Remove(GetType().Name.Length - 4)
        : GetType().Name;
}

public abstract class AwsItem<T> : Item<T> where T : class
{
    protected AwsItem(string parentPath, T underlyingObject) : base(parentPath, underlyingObject)
    {
    }
    public override string TypeName => GetType().FullName!;

    public override string ItemType => GetType().Name.EndsWith("Item")
        ? GetType().Name.Remove(GetType().Name.Length - 4)
        : GetType().Name;
}