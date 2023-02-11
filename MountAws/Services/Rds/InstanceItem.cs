using Amazon.RDS.Model;
using MountAnything;

namespace MountAws.Services.Rds;

public class InstanceItem : AwsItem<DBInstance>
{
    public InstanceItem(ItemPath parentPath, DBInstance dbInstance) : base(parentPath, dbInstance)
    {
        ItemName = dbInstance.DBInstanceIdentifier;
    }

    public override string ItemName { get; }
    public override bool IsContainer => false;
    public override string? WebUrl =>
        UrlBuilder.CombineWith($"rds/home#database:id={UnderlyingObject.DBInstanceIdentifier};is-cluster=false");
}