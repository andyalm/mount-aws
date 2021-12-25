using MountAnything;
using MountAws.Api.Elbv2;
using MountAws.Services.Core;

namespace MountAws.Services.Elbv2;

public class RulesHandler : PathHandler
{
    private readonly IElbv2Api _elbv2;

    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "rules",
            "List the rules attached to the load balancer listener");
    }
    
    public RulesHandler(string path, IPathHandlerContext context, IElbv2Api elbv2) : base(path, context)
    {
        _elbv2 = elbv2;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var listenerHandler = new ListenerHandler(ParentPath, Context, _elbv2);
        var listener = listenerHandler.GetItem() as ListenerItem;
        if (listener == null)
        {
            return Enumerable.Empty<Item>();
        }

        return _elbv2.DescribeRules(listener.ListenerArn).Select(r => new RuleItem(Path, r));
    }
}