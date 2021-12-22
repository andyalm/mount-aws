using System.Management.Automation;
using MountAnything;
using MountAws.Api;

namespace MountAws;

public abstract class AwsItem : Item
{
    public PSObject Object { get; }
    
    protected AwsItem(string parentPath, PSObject underlyingObject) : base(parentPath)
    {
        UnderlyingObject = underlyingObject;
        Object = underlyingObject;
    }

    protected T? Property<T>(string name)
    {
        return Object.Property<T>(name);
    }
    
    public override object UnderlyingObject { get; }
}