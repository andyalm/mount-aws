namespace MountAws;

public class ProfilesRoot : AwsItem
{
    public override string FullPath => "/";
    public override object UnderlyingObject => new object();
    public override string ItemType => "Root";
    public override bool IsContainer => true;
}