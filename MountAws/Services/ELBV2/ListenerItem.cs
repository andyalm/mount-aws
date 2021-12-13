using Amazon.ElasticLoadBalancingV2.Model;
using MountAnything;

namespace MountAws.Services.ELBV2;

public class ListenerItem : Item
{
    public Listener Listener { get; }

    public ListenerItem(string parentPath, Listener listener) : base(parentPath)
    {
        Listener = listener;
    }

    public override string ItemName => Listener.Port.ToString();
    public override object UnderlyingObject => Listener;
    public override string ItemType => "Listener";
    public override bool IsContainer => true;
}