using Amazon.IdentityManagement;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Iam;

public class RoleStatementsHandler : PathHandler
{
    private readonly IAmazonIdentityManagementService _iam;
    private readonly IItemAncestor<RoleItem> _role;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "statements",
            "Lists all of the statements from all of the policies associated with the current role");
    }
    
    public RoleStatementsHandler(ItemPath path, IPathHandlerContext context, IAmazonIdentityManagementService iam, IItemAncestor<RoleItem> role) : base(path, context)
    {
        _iam = iam;
        _role = role;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _iam.ListRolePolicies(_role.Item.ItemName)
            .SelectMany(p => p.Statements())
            .Concat(_iam.ListAttachedRolePolicies(_role.Item.ItemName)
                .SelectMany(p => p.Statements()))
            .Select((s, index) => new StatementItem(Path, s, index));
    }
}