using MountAnything;
using MountAws.Api.Ec2;
using MountAws.Api.Elbv2;

namespace MountAws.Services.Elbv2;

public class DefaultActionHandler : PathHandler
{
    private readonly IElbv2Api _elbv2;
    private readonly IEc2Api _ec2;

    public DefaultActionHandler(string path, IPathHandlerContext context, IElbv2Api elbv2, IEc2Api ec2) : base(path, context)
    {
        _elbv2 = elbv2;
        _ec2 = ec2;
    }

    protected override IItem? GetItemImpl()
    {
        var defaultActionsHandler = new DefaultActionsHandler(ParentPath, Context, _elbv2, _ec2);
        return defaultActionsHandler
            .GetChildItems()
            .SingleOrDefault(i => i.ItemName.Equals(ItemName, StringComparison.OrdinalIgnoreCase));
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