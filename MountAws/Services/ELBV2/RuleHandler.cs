using MountAnything;
using MountAws.Api.Elbv2;

namespace MountAws.Services.ELBV2;

public class RuleHandler : PathHandler
{
    private readonly IElbv2Api _elbv2;

    public RuleHandler(string path, IPathHandlerContext context, IElbv2Api elbv2) : base(path, context)
    {
        _elbv2 = elbv2;
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

        return rule.Actions.Select(a => ActionItem.Create(Path, a));
    }
}