using Amazon;

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

    protected override AwsItem? GetItemImpl()
    {
        var regionEndpoint = RegionEndpoint.GetBySystemName(ItemName);
        if (regionEndpoint != null && (regionEndpoint.SystemName == "global" || regionEndpoint.DisplayName != "Unknown"))
        {
            return new AwsRegion(ParentPath, regionEndpoint);
        }

        return null;
    }

    protected override IEnumerable<AwsItem> GetChildItemsImpl()
    {
        yield return new GenericContainerItem(Path, "ec2");
    }
}