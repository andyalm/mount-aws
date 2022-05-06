using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.Iam;

public class StatementItem : AwsItem
{
    public StatementItem(ItemPath parentPath, PSObject statement, int index) : base(parentPath, statement)
    {
        var sid = statement.Property<string>("Sid");
        
        ItemName = string.IsNullOrEmpty(sid) ? index.ToString() : sid;
    }

    public override string ItemName { get; }
    public override bool IsContainer => false;
}