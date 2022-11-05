using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;
using MountAnything;

namespace MountAws.Services.Iam;

public class RoleNavigator : ItemNavigator<Role, RoleItem>
{
    private readonly IAmazonIdentityManagementService _iam;

    public RoleNavigator(IAmazonIdentityManagementService iam)
    {
        _iam = iam;
    }

    protected override RoleItem CreateDirectoryItem(ItemPath parentPath, ItemPath directoryPath)
    {
        return new RoleItem(parentPath, directoryPath.ToString());
    }

    protected override RoleItem CreateItem(ItemPath parentPath, Role model)
    {
        return new RoleItem(parentPath, model);
    }

    protected override ItemPath GetPath(Role model)
    {
        var directory = string.IsNullOrEmpty(model.Path) ? ItemPath.Root : new(model.Path);
        return directory.Combine(model.RoleName);
    }

    protected override IEnumerable<Role> ListItems(ItemPath? pathPrefix)
    {
        return _iam.ListRoles(pathPrefix?.ToString());
    }
}