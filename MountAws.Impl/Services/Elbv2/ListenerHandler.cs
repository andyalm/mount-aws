using Amazon.EC2;
using MountAnything;
using MountAws.Api;
using MountAws.Api.Ec2;
using MountAws.Api.Elbv2;

namespace MountAws.Services.Elbv2;

public class ListenerHandler : PathHandler
{
    private readonly IElbv2Api _elbv2;
    private readonly IAmazonEC2 _ec2;

    public ListenerHandler(string path, IPathHandlerContext context, IElbv2Api elbv2, IAmazonEC2 ec2) : base(path, context)
    {
        _elbv2 = elbv2;
        _ec2 = ec2;
    }

    protected override IItem? GetItemImpl()
    {
        var loadBalancerHandler = new LoadBalancerHandler(ParentPath, Context, _elbv2, _ec2);
        var loadBalancerItem = loadBalancerHandler.GetItem() as LoadBalancerItem;
        if (loadBalancerItem == null)
        {
            return null;
        }

        var listener = _elbv2.DescribeListeners(loadBalancerItem.LoadBalancerArn)
            .SingleOrDefault(l => l.Property<string>("Port") == ItemName);
        if (listener != null)
        {
            return new ListenerItem(ParentPath, listener);
        }

        return null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return DefaultActionsHandler.CreateItem(Path);
        yield return RulesHandler.CreateItem(Path);
    }
}