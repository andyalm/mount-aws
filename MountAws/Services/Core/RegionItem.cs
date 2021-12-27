using System.Management.Automation;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws;

public class RegionItem : AwsItem
{
    public RegionItem(string parentPath, PSObject regionEndpoint) :  base(parentPath, regionEndpoint)
    {
        
    }

    public override string ItemName => Property<string>("SystemName")!;
    public override string ItemType => CoreItemTypes.Region;
    public override bool IsContainer => true;
}