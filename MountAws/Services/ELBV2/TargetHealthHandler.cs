using Amazon.ElasticLoadBalancingV2;

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

    protected override AwsItem? GetItemImpl()
    {
        var targetGroupHandler = new TargetGroupHandler(ParentPath, Context, _elbv2);
        return targetGroupHandler.GetChildItems().SingleOrDefault(i => i.ItemName == ItemName);
    }

    protected override IEnumerable<AwsItem> GetChildItemsImpl()
    {
        yield break;
    }
}