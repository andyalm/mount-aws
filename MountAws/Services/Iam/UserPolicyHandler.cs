using System.Management.Automation.Provider;
using Amazon.IdentityManagement;
using MountAnything;
using MountAnything.Content;

namespace MountAws.Services.Iam;

public class UserPolicyHandler : PathHandler, IContentReaderHandler
{
    private readonly UserPoliciesHandler _parentHandler;

    public UserPolicyHandler(ItemPath path, IPathHandlerContext context, IAmazonIdentityManagementService iam, IItemAncestor<UserItem> user) : base(path, context)
    {
        _parentHandler = new UserPoliciesHandler(path.Parent, context, iam, user);
    }

    protected override IItem? GetItemImpl()
    {
        return _parentHandler.GetChildItems()
            .SingleOrDefault(i => i.ItemName.Equals(ItemName, StringComparison.OrdinalIgnoreCase));
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }

    public IContentReader GetContentReader()
    {
        var item = GetItem() as EntityPolicyItem;
        if (item == null)
        {
            throw new InvalidOperationException("Item does not exist");
        }

        return new StringContentReader(item.RawDocument);
    }
}