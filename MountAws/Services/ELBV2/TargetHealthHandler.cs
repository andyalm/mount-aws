using Amazon.ElasticLoadBalancingV2;
using MountAnything;

namespace MountAws.Services.ELBV2;

public class TargetHealthHandler : PathHandler
{
    private readonly IAmazonElasticLoadBalancingV2 _elbv2;

    public TargetHealthHandler(string path, IPathHandlerContext context, IAmazonElasticLoadBalancingV2 elbv2) : base(path, context)
    {
        _elbv2 = elbv2;
    }

    protected override bool ExistsImpl()
    {
        return GetItem() != null;
    }

    protected override Item? GetItemImpl()
    {
        var targetGroupHandler = new TargetGroupHandler(ParentPath, Context, _elbv2);
        return targetGroupHandler.GetChildItems().SingleOrDefault(i => i.ItemName == ItemName);
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        yield break;
    }
}