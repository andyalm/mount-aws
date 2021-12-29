using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.Elbv2;

public class ListenerItem : AwsItem
{
    public ListenerItem(string parentPath, PSObject listener) : base(parentPath, listener) {}

    public override string ItemName => Property<string>("Port")!;
    public override string ItemType => Elbv2ItemTypes.Listener;
    public override bool IsContainer => true;
    public int Port => Property<int>("Port");
    public string ListenerArn => Property<string>("ListenerArn")!;
    public IEnumerable<PSObject> DefaultActions => Property<IEnumerable<PSObject>>("DefaultActions")!;
}