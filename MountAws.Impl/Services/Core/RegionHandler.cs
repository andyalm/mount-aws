using Amazon;
using MountAnything;
using MountAws.Api;
using MountAws.Services.Core;
using MountAws.Services.Ec2;
using MountAws.Services.Ecr;
using MountAws.Services.Ecs;
using MountAws.Services.Elbv2;
using MountAws.Services.Route53;
using MountAws.Services.S3;

namespace MountAws;

public class RegionHandler : PathHandler
{
    public RegionHandler(ItemPath path, IPathHandlerContext context) : base(path, context)
    {
       
    }

    protected override IItem? GetItemImpl()
    {
        var regionEndpoint = RegionEndpoint.GetBySystemName(ItemName);
        if (regionEndpoint.DisplayName != "Unknown")
        {
            return new RegionItem(ParentPath, regionEndpoint);
        }
        
        return null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return Ec2RootHandler.CreateItem(Path);
        yield return EcrRootHandler.CreateItem(Path);
        yield return ECSRootHandler.CreateItem(Path);
        yield return Elbv2RootHandler.CreateItem(Path);
        yield return Route53RootHandler.CreateItem(Path);
        yield return S3RootHandler.CreateItem(Path);
    }
}