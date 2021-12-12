using Amazon.ElasticLoadBalancingV2.Model;

namespace MountAws.Services.ELBV2;

public class ListenerItem : AwsItem
{
    private readonly string _parentPath;
    public Listener Listener { get; }

    public ListenerItem(string parentPath, Listener listener)
    {
        _parentPath = parentPath;
        Listener = listener;
    }

    public override string FullPath => AwsPath.Combine(_parentPath, ItemName);
    public override string ItemName => Listener.Port.ToString();
    public override object UnderlyingObject => Listener;
    public override string ItemType => "Listener";
    public override bool IsContainer => true;
}