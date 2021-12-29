using MountAnything;
using MountAws.Api.Elbv2;
using TargetGroupNotFoundException = MountAws.Api.Elbv2.TargetGroupNotFoundException;

namespace MountAws.Services.Elbv2;

public class TargetGroupHandler : PathHandler, IRemoveItemHandler
{
    private readonly IElbv2Api _elbv2;

    public TargetGroupHandler(string path, IPathHandlerContext context, IElbv2Api elbv2) : base(path, context)
    {
        _elbv2 = elbv2;
    }

    protected override IItem? GetItemImpl()
    {
        try
        {
            var targetGroup = _elbv2.GetTargetGroup(ItemName);
            return new TargetGroupItem(ParentPath, targetGroup);
        }
        catch (TargetGroupNotFoundException)
        {
            return null;
        }
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var targetGroupItem = GetItem() as TargetGroupItem;
        if (targetGroupItem == null)
        {
            return Enumerable.Empty<Item>();
        }
        return _elbv2.DescribeTargetHealth(targetGroupItem.TargetGroupArn).Select(d => new TargetHealthItem(Path, d));
    }

    public void RemoveItem()
    {
        var item = GetItem(Freshness.Default) as TargetGroupItem;
        if (item == null)
        {
            throw new InvalidOperationException($"The target group '{ItemName}' could not be found");
        }
        
        _elbv2.DeleteTargetGroup(item.TargetGroupArn);
    }
}