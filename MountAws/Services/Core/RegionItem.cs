using System.Management.Automation;
using MountAnything;

namespace MountAws;

public class RegionItem : Item
{
    public RegionItem(string parentPath, PSObject regionEndpoint) :  base(parentPath, regionEndpoint)
    {
        
    }

    public override string ItemName => Property<string>("SystemEndpoint")!;
    public override string ItemType => "Region";
    public override bool IsContainer => true;
}