using Amazon.IdentityManagement;
using MountAnything;

namespace MountAws.Services.Iam;

public class UserStatementHandler : PathHandler
{
    private readonly UserStatementsHandler _parentHandler;
    
    public UserStatementHandler(ItemPath path, IPathHandlerContext context, IAmazonIdentityManagementService iam, IItemAncestor<UserItem> user) : base(path, context)
    {
        _parentHandler = new UserStatementsHandler(path.Parent, context, iam, user);
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