using System.Management.Automation;
using Amazon.ApplicationAutoScaling;
using MountAnything;

namespace MountAws.Services.Autoscaling;

public class ServiceItem : AwsItem<ServiceNamespace>
{
    public ServiceItem(ItemPath parentPath, ServiceNamespace underlyingObject) : base(parentPath, underlyingObject)
    {
        
    }

    public override string ItemName => UnderlyingObject.Value;
    public override bool IsContainer => true;
}