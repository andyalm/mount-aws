using Amazon.ElasticLoadBalancingV2.Model;
using MountAnything;
using Action = Amazon.ElasticLoadBalancingV2.Model.Action;

namespace MountAws.Services.Elbv2;

public class ListenerItem : AwsItem<Listener>
{
    public ListenerItem(ItemPath parentPath, Listener listener) : base(parentPath, listener) {}

    public override string ItemName => UnderlyingObject.Port.ToString();
    public override string ItemType => Elbv2ItemTypes.Listener;
    public override bool IsContainer => true;
    public int Port => UnderlyingObject.Port;
    public string ListenerArn => UnderlyingObject.ListenerArn;
    public IEnumerable<Action> DefaultActions => UnderlyingObject.DefaultActions;
}