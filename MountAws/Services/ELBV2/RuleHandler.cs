using Amazon.ElasticLoadBalancingV2;
using MountAnything;

namespace MountAws.Services.ELBV2;

public class RuleHandler : PathHandler
{
    private readonly IAmazonElasticLoadBalancingV2 _elbv2;

    public RuleHandler(string path, IPathHandlerContext context, IAmazonElasticLoadBalancingV2 elbv2) : base(path, context)
    {
        _elbv2 = elbv2;
    }

    protected override bool ExistsImpl()
    {
        return GetItem() != null;
    }

    protected override Item? GetItemImpl()
    {
        var rulesHandler = new RulesHandler(ParentPath, Context, _elbv2);
        return rulesHandler.GetChildItems().FirstOrDefault(i => i.ItemName == ItemName) as RuleItem;
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        var rule = GetItem() as RuleItem;
        if (rule == null)
        {
            return Enumerable.Empty<Item>();
        }

        return rule.Rule.Actions.Select(a => ActionItem.Create(Path, a));
    }
}