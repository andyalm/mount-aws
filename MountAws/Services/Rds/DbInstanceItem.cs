using Amazon.RDS.Model;
using MountAnything;

namespace MountAws.Services.Rds;

public class DbInstanceItem : AwsItem<DBInstance>
{
    public DbInstanceItem(ItemPath parentPath, DBInstance dbInstance) : base(parentPath, dbInstance)
    {
        ItemName = dbInstance.DBInstanceIdentifier;
    }

    public override string ItemName { get; }
    
    public override bool IsContainer => true;
}