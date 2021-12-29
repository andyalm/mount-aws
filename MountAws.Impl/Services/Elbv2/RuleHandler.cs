using Amazon.EC2;
using MountAnything;
using MountAws.Api.Ec2;
using MountAws.Api.Elbv2;

namespace MountAws.Services.Elbv2;

public class RuleHandler : PathHandler, IRemoveItemHandler
{
    private readonly IElbv2Api _elbv2;
    private readonly IAmazonEC2 _ec2;

    public RuleHandler(string path, IPathHandlerContext context, IElbv2Api elbv2, IAmazonEC2 ec2) : base(path, context)
    {
        _elbv2 = elbv2;
        _ec2 = ec2;
    }

    protected override IItem? GetItemImpl()
    {
        var rulesHandler = new RulesHandler(ParentPath, Context, _elbv2, _ec2);
        return rulesHandler.GetChildItems().FirstOrDefault(i => i.ItemName == ItemName) as RuleItem;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var rule = GetItem() as RuleItem;
        if (rule == null)
        {
            return Enumerable.Empty<Item>();
        }

        return rule.Actions.Select(a => ActionItem.Create(Path, a));
    }

    public void RemoveItem()
    {
        var rule = GetItem() as RuleItem;
        if (rule == null)
        {
            throw new InvalidOperationException($"The rule '{ItemName}' does not exist");
        }
        
        _elbv2.DeleteRule(rule.RuleArn);
    }
}