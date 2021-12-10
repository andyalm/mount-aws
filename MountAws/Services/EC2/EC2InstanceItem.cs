using Amazon.EC2.Model;

namespace MountAws.Services.EC2;

public class EC2InstanceItem : AwsItem
{
    private readonly Instance _ec2Instance;
    
    public EC2InstanceItem(string parentPath, Instance instance, string? itemName = null)
    {
        ParentPath = parentPath;
        _ec2Instance = instance;
        ItemName = itemName ?? _ec2Instance.InstanceId;
    }

    public string ParentPath { get; }
    public override string FullPath => AwsPath.Combine(ParentPath, ItemName);
    public override string ItemName { get; }
    public override object UnderlyingObject => _ec2Instance;
    public override string ItemType => "EC2Instance";
    public override bool IsContainer => false;
}