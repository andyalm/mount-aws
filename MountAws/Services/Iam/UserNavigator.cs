using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;
using MountAnything;

namespace MountAws.Services.Iam;

public class UserNavigator : ItemNavigator<User, UserItem>
{
    private readonly IAmazonIdentityManagementService _iam;

    public UserNavigator(IAmazonIdentityManagementService iam)
    {
        _iam = iam;
    }
    protected override UserItem CreateDirectoryItem(ItemPath parentPath, ItemPath directoryPath)
    {
        return new UserItem(parentPath, directoryPath.ToString());
    }

    protected override UserItem CreateItem(ItemPath parentPath, User model)
    {
        return new UserItem(parentPath, model);
    }

    protected override ItemPath GetPath(User model)
    {
        var directory = string.IsNullOrEmpty(model.Path) ? ItemPath.Root : new(model.Path);
        return directory.Combine(model.UserName);
    }

    protected override IEnumerable<User> ListItems(ItemPath? pathPrefix)
    {
        return _iam.ListUsers(pathPrefix?.ToString());
    }
}