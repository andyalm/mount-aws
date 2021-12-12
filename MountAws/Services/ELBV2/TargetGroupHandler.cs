using Amazon.ElasticLoadBalancingV2;
using Amazon.ElasticLoadBalancingV2.Model;

namespace MountAws.Services.ELBV2;

public class TargetGroupHandler : PathHandler
{
    private readonly IAmazonElasticLoadBalancingV2 _elbv2;

    public TargetGroupHandler(string path, IPathHandlerContext context, IAmazonElasticLoadBalancingV2 elbv2) : base(path, context)
    {
        _elbv2 = elbv2;
    }

    protected override bool ExistsImpl()
    {
        return GetItem() != null;
    }

    protected override AwsItem? GetItemImpl()
    {
        var targetGroup = _elbv2.GetTargetGroup(ItemName);
        if (targetGroup != null)
        {
            return new TargetGroupItem(ParentPath, targetGroup);
        }

        return null;
    }

    protected override IEnumerable<AwsItem> GetChildItemsImpl()
    {
        var targetGroupItem = GetItem() as TargetGroupItem;
        if (targetGroupItem == null)
        {
            return Enumerable.Empty<AwsItem>();
        }
        var response = _elbv2.DescribeTargetHealthAsync(new DescribeTargetHealthRequest
        {
            TargetGroupArn = targetGroupItem.TargetGroup.TargetGroupArn,
        }).GetAwaiter().GetResult();

        return response.TargetHealthDescriptions.Select(d => new TargetHealthItem(Path, d));
    }
}