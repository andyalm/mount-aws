using MountAnything;
using MountAws.Api.Elbv2;

namespace MountAws.Services.ELBV2;

public class TargetHealthHandler : PathHandler
{
    private readonly IElbv2Api _elbv2;

    public TargetHealthHandler(string path, IPathHandlerContext context, IElbv2Api elbv2) : base(path, context)
    {
        _elbv2 = elbv2;
    }

    protected override Item? GetItemImpl()
    {
        var targetGroupHandler = new TargetGroupHandler(ParentPath, Context, _elbv2);
        return targetGroupHandler.GetChildItems().SingleOrDefault(i => i.ItemName == ItemName);
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        yield break;
    }
}