using Amazon;
using MountAnything;
using MountAws.Api;
using MountAws.Services.EC2;
using MountAws.Services.ECR;
using MountAws.Services.ECS;
using MountAws.Services.ELBV2;
using MountAws.Services.S3;

namespace MountAws;

public class RegionHandler : PathHandler
{
    private readonly ICoreApi _api;

    public RegionHandler(string path, IPathHandlerContext context, ICoreApi api) : base(path, context)
    {
        _api = api;
    }

    protected override Item? GetItemImpl()
    {
        if (_api.TryGetRegion(ItemName, out var region) && region.Property<string>("DisplayName") != "Unknown")
        {
            return new RegionItem(ParentPath, region);
        }
        
        return null;
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        yield return EC2Handler.CreateItem(Path);
        yield return ECRRootHandler.CreateItem(Path);
        yield return ECSRootHandler.CreateItem(Path);
        yield return ELBV2Handler.CreateItem(Path);
        yield return S3RootHandler.CreateItem(Path);
    }
}