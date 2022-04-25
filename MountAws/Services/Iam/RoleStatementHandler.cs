using Amazon.IdentityManagement;
using MountAnything;

namespace MountAws.Services.Iam;

public class RoleStatementHandler : PathHandler
{
    private readonly RoleStatementsHandler _parentHandler;
    
    public RoleStatementHandler(ItemPath path, IPathHandlerContext context, IAmazonIdentityManagementService iam, IItemAncestor<RoleItem> role) : base(path, context)
    {
        _parentHandler = new RoleStatementsHandler(path.Parent, context, iam, role);
    }

    protected override IItem? GetItemImpl()
    {
        return _parentHandler.GetChildItems()
            .Cast<StatementItem>()
            .SingleOrDefault(i => i.ItemName.Equals(ItemName, StringComparison.OrdinalIgnoreCase));
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}