using MountAnything;
using MountAws.Api;
using MountAws.Services.Ec2;
using MountAws.Services.Ecr;
using MountAws.Services.Ecs;
using MountAws.Services.Elbv2;
using MountAws.Services.S3;

namespace MountAws;

public class RegionHandler : PathHandler
{
    private readonly ICoreApi _api;

    public RegionHandler(string path, IPathHandlerContext context, ICoreApi api) : base(path, context)
    {
        _api = api;
    }

    protected override IItem? GetItemImpl()
    {
        if (_api.TryGetRegion(ItemName, out var region) && region.Property<string>("DisplayName") != "Unknown")
        {
            return new RegionItem(ParentPath, region);
        }
        
        return null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return Ec2RootHandler.CreateItem(Path);
        yield return EcrRootHandler.CreateItem(Path);
        yield return ECSRootHandler.CreateItem(Path);
        yield return Elbv2RootHandler.CreateItem(Path);
        yield return S3RootHandler.CreateItem(Path);
    }
}