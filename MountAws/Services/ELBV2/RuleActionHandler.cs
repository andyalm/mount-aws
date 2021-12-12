using Amazon.ElasticLoadBalancingV2;

namespace MountAws.Services.ELBV2;

public class RuleActionHandler : PathHandler
{
    private readonly IAmazonElasticLoadBalancingV2 _elbv2;

    public RuleActionHandler(string path, IPathHandlerContext context, IAmazonElasticLoadBalancingV2 elbv2) : base(path, context)
    {
        _elbv2 = elbv2;
    }

    protected override bool ExistsImpl()
    {
        return GetItem() != null;
    }

    protected override AwsItem? GetItemImpl()
    {
        var ruleHandler = new RuleHandler(ParentPath, Context, _elbv2);
        return ruleHandler.GetChildItems().SingleOrDefault(r => r.ItemName == ItemName);
    }

    protected override IEnumerable<AwsItem> GetChildItemsImpl()
    {
        if (GetItem() is ActionItem item)
        {
            return item.GetChildren(_elbv2);
        }
        
        return Enumerable.Empty<AwsItem>();
    }
}