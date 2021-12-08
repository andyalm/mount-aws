namespace MountAws;

public class ProfilesRoot : AwsItem
{
    public override string FullPath => "/";

    public override string Name => "";
    public override object UnderlyingObject => new();
    public override string ItemType => "Root";
    public override bool IsContainer => true;
}