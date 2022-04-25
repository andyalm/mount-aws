using Amazon.Route53;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Route53;

public class HostedZonesHandler : PathHandler
{
    private readonly IAmazonRoute53 _route53;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "hosted-zones",
            "Contains hosted zones and child dns records");
    }
    
    public HostedZonesHandler(ItemPath path, IPathHandlerContext context, IAmazonRoute53 route53) : base(path, context)
    {
        _route53 = route53;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _route53.ListHostedZones()
            .Select(z => new HostedZoneItem(Path, z))
            .OrderBy(i => i.Name);
    }
}