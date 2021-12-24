using MountAnything;
using MountAws.Api.Elbv2;
using MountAws.Services.Core;

namespace MountAws.Services.Elbv2;

public class DefaultActionsHandler : PathHandler
{
    private readonly IElbv2Api _elbv2;

    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "default-actions",
            "List the default actions for the load balancer listener");
    }

    public DefaultActionsHandler(string path, IPathHandlerContext context, IElbv2Api elbv2) : base(path, context)
    {
        _elbv2 = elbv2;
    }

    protected override Item? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        var listenerHandler = new ListenerHandler(ParentPath, Context, _elbv2);
        var listener = listenerHandler.GetItem() as ListenerItem;
        if (listener == null)
        {
            return Enumerable.Empty<Item>();
        }

        return listener.DefaultActions
            .Select(a => ActionItem.Create(Path, a));
    }
}