using Amazon.IdentityManagement;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Iam;

public class UserPoliciesHandler : PathHandler
{
    private readonly IAmazonIdentityManagementService _iam;
    private readonly IItemAncestor<UserItem> _user;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "policies",
            "Lists the policies embedded and attached to the parent user");
    }
    
    public UserPoliciesHandler(ItemPath path, IPathHandlerContext context, IAmazonIdentityManagementService iam, IItemAncestor<UserItem> user) : base(path, context)
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
            .Select(p => new EntityPolicyItem(Path, p))
            .Concat(_iam.ListAttachedUserPolicies(_user.Item.ItemName)
                .Select(p => new EntityPolicyItem(Path, p)));
    }
}