using MountAnything;
using MountAws.Api.Route53;

namespace MountAws.Services.Route53;

public class ResourceRecordHandler : PathHandler
{
    private readonly IRoute53Api _route53;

    public ResourceRecordHandler(string path, IPathHandlerContext context, IRoute53Api route53) : base(path, context)
    {
        _route53 = route53;
    }

    protected override IItem? GetItemImpl()
    {
        var hostedZoneId = ItemPath.GetLeaf(ParentPath);
        try
        {
            var resourceRecord = _route53.GetResourceRecordSet(hostedZoneId, ItemName);
            return new ResourceRecordItem(ParentPath, resourceRecord);
        }
        catch (RecordSetNotFoundException)
        {
            return null;
        }
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}