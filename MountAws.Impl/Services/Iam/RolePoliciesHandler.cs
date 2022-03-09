using System.Management.Automation;
using System.Security.Cryptography;
using Amazon.IdentityManagement;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Iam;

public class RolePoliciesHandler : PathHandler
{
    private readonly IAmazonIdentityManagementService _iam;
    private readonly IItemAncestor<RoleItem> _role;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "policies",
            "Lists the policies embedded and attached to the parent role");
    }
    
    public RolePoliciesHandler(ItemPath path, IPathHandlerContext context, IAmazonIdentityManagementService iam, IItemAncestor<RoleItem> role) : base(path, context)
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
            .Select(p => new RolePolicyItem(Path, p))
            .Concat(_iam.ListAttachedRolePolicies(_role.Item.ItemName)
                .Select(p => new RolePolicyItem(Path, p)));
    }
}