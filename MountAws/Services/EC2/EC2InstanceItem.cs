using Amazon.EC2.Model;

namespace MountAws.Services.EC2;

public class EC2InstanceItem : AwsItem
{
    private readonly Instance _ec2Instance;
    
    public EC2InstanceItem(string parentPath, Instance instance)
    {
        ParentPath = parentPath;
        _ec2Instance = instance;
    }

    public string ParentPath { get; }
    public override string FullPath => AwsPath.Combine(ParentPath, ItemName);
    public override string ItemName => _ec2Instance.InstanceId;
    public override object UnderlyingObject => _ec2Instance;
    public override string ItemType => "EC2Instance";
    public override bool IsContainer => false;
}