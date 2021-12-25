using MountAnything;
using MountAws.Api.Elbv2;

namespace MountAws.Services.Elbv2;

public class RuleActionHandler : PathHandler
{
    private readonly IElbv2Api _elbv2;

    public RuleActionHandler(string path, IPathHandlerContext context, IElbv2Api elbv2) : base(path, context)
    {
        _elbv2 = elbv2;
    }

    protected override IItem? GetItemImpl()
    {
        var ruleHandler = new RuleHandler(ParentPath, Context, _elbv2);
        return ruleHandler.GetChildItems().SingleOrDefault(r => r.ItemName == ItemName);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        if (GetItem() is ActionItem item)
        {
            return item.GetChildren(_elbv2);
        }
        
        return Enumerable.Empty<Item>();
    }
}