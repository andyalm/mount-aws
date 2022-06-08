using Amazon.IdentityManagement;
using MountAnything;

namespace MountAws.Services.Iam;

public class UserHandler : PathHandler
{
    private readonly IAmazonIdentityManagementService _iam;
    private readonly UserNavigator _navigator;
    private readonly IamItemPath _userPath;

    public UserHandler(ItemPath path, IPathHandlerContext context, IAmazonIdentityManagementService iam, UserNavigator navigator, IamItemPath userPath) : base(path, context)
    {
        _iam = iam;
        _navigator = navigator;
        _userPath = userPath;
    }

    protected override IItem? GetItemImpl()
    {
        var role = _iam.GetUserOrDefault(ItemName);
        if (role != null)
        {
            return new UserItem(ParentPath, role);
        }

        return new UserItem(ParentPath, ItemName);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return GetItem() switch
        {
            UserItem { ItemType: IamItemTypes.Directory } => GetChildUsersWithinDirectory(),
            UserItem { ItemType: IamItemTypes.User } => GetUserChildren(),
            _ => Enumerable.Empty<IItem>()
        };
    }

    private IEnumerable<IItem> GetUserChildren()
    {
        yield return RolePoliciesHandler.CreateItem(Path);
        yield return RoleStatementsHandler.CreateItem(Path);
    }

    private IEnumerable<IItem> GetChildUsersWithinDirectory()
    {
        return _navigator.ListChildItems(Path, _userPath.Path);
    }
}