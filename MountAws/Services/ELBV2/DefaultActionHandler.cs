using MountAnything;
using MountAws.Api.Elbv2;

namespace MountAws.Services.ELBV2;

public class DefaultActionHandler : PathHandler
{
    private readonly IElbv2Api _elbv2;

    public DefaultActionHandler(string path, IPathHandlerContext context, IElbv2Api elbv2) : base(path, context)
    {
        _elbv2 = elbv2;
    }

    protected override Item? GetItemImpl()
    {
        var defaultActionsHandler = new DefaultActionsHandler(ParentPath, Context, _elbv2);
        return defaultActionsHandler
            .GetChildItems()
            .SingleOrDefault(i => i.ItemName.Equals(ItemName, StringComparison.OrdinalIgnoreCase));
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        if (GetItem() is ActionItem item)
        {
            return item.GetChildren(_elbv2);
        }
        
        return Enumerable.Empty<Item>();
    }
}