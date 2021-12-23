using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.Ecs;

public class ServiceItem : Item
{
    public ServiceItem(string parentPath, PSObject service) : base(parentPath, service)
    {
        ItemName = Property<string>("ServiceName")!;
    }

    public override string ItemName { get; }
    public override string ItemType => EcsItemTypes.Service;
    public override bool IsContainer => true;
}