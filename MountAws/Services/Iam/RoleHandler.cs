using Amazon.IdentityManagement;
using MountAnything;

namespace MountAws.Services.Iam;

public class RoleHandler : PathHandler
{
    private readonly IAmazonIdentityManagementService _iam;
    private readonly IamItemPath _rolePath;

    public RoleHandler(ItemPath path, IPathHandlerContext context, IAmazonIdentityManagementService iam, IamItemPath rolePath) : base(path, context)
    {
        _iam = iam;
        _rolePath = rolePath;
    }

    protected override IItem? GetItemImpl()
    {
        var role = _iam.GetRoleOrDefault(ItemName);
        if (role != null)
        {
            return new RoleItem(ParentPath, role);
        }

        return new RoleItem(ParentPath, ItemName);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return GetItem() switch
        {
            RoleItem { ItemType: IamItemTypes.Directory } => GetChildRolesWithinDirectory(),
            RoleItem { ItemType: IamItemTypes.Role } => GetRoleChildren(),
            _ => Enumerable.Empty<IItem>()
        };
    }

    private IEnumerable<IItem> GetRoleChildren()
    {
        yield return RolePoliciesHandler.CreateItem(Path);
        yield return RoleStatementsHandler.CreateItem(Path);
    }

    private IEnumerable<IItem> GetChildRolesWithinDirectory()
    {
        return _iam.ListChildRoleItems(Path, _rolePath.ToString());
    }
}