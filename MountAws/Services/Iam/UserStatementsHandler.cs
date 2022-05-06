using Amazon.IdentityManagement;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Iam;

public class UserStatementsHandler : PathHandler
{
    private readonly IAmazonIdentityManagementService _iam;
    private readonly IItemAncestor<UserItem> _user;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "statements",
            "Lists all of the statements from all of the policies associated with the current user");
    }
    
    public UserStatementsHandler(ItemPath path, IPathHandlerContext context, IAmazonIdentityManagementService iam, IItemAncestor<UserItem> user) : base(path, context)
    {
        _iam = iam;
        _user = user;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _iam.ListUserPolicies(_user.Item.ItemName)
            .SelectMany(p => p.Statements())
            .Concat(_iam.ListAttachedUserPolicies(_user.Item.ItemName)
                .SelectMany(p => p.Statements()))
            .Select((s, index) => new StatementItem(Path, s, index));
    }
}