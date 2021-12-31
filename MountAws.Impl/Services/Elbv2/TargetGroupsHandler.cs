using Amazon.ElasticLoadBalancingV2;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Elbv2;

public class TargetGroupsHandler : PathHandler
{
    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "target-groups",
            "Navigate the target groups in the current account and region");
    }
    
    private readonly IAmazonElasticLoadBalancingV2 _elbv2;

    public TargetGroupsHandler(ItemPath path, IPathHandlerContext context, IAmazonElasticLoadBalancingV2 elbv2) : base(path, context)
    {
        _elbv2 = elbv2;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _elbv2.DescribeTargetGroups()
            .Select(t => new TargetGroupItem(Path, t))
            .OrderBy(t => t.ItemName);
    }
}