using Amazon;
using MountAnything;
using MountAws.Services.EC2;
using MountAws.Services.ECS;
using MountAws.Services.ELBV2;

namespace MountAws;

public class RegionHandler : PathHandler
{
    public RegionHandler(string path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override bool ExistsImpl()
    {
        return GetItem() != null;
    }

    protected override Item? GetItemImpl()
    {
        var regionEndpoint = RegionEndpoint.GetBySystemName(ItemName);
        if (regionEndpoint != null && (regionEndpoint.SystemName == "global" || regionEndpoint.DisplayName != "Unknown"))
        {
            return new AwsRegion(ParentPath, regionEndpoint);
        }

        return null;
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        yield return EC2Handler.CreateItem(Path);
        yield return ECSRootHandler.CreateItem(Path);
        yield return ELBV2Handler.CreateItem(Path);
    }
}