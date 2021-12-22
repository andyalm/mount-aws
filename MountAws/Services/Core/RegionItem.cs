using System.Management.Automation;

namespace MountAws;

public class RegionItem : AwsItem
{
    public RegionItem(string parentPath, PSObject regionEndpoint) :  base(parentPath, regionEndpoint)
    {
        
    }

    public override string ItemName => Property<string>("SystemEndpoint")!;
    public override string ItemType => "Region";
    public override bool IsContainer => true;
}