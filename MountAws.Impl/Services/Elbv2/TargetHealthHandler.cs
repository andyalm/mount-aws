using MountAnything;
using MountAws.Api.Elbv2;

namespace MountAws.Services.Elbv2;

public class TargetHealthHandler : PathHandler
{
    private readonly IElbv2Api _elbv2;

    public TargetHealthHandler(string path, IPathHandlerContext context, IElbv2Api elbv2) : base(path, context)
    {
        _elbv2 = elbv2;
    }

    protected override IItem? GetItemImpl()
    {
        var targetGroupHandler = new TargetGroupHandler(ParentPath, Context, _elbv2);
        return targetGroupHandler.GetChildItems().SingleOrDefault(i => i.ItemName == ItemName);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}