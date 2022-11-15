using System.Management.Automation.Provider;
using System.Net;
using System.Text;
using Amazon.IdentityManagement;
using MountAnything;
using MountAnything.Content;

namespace MountAws.Services.Iam;

public class RolePolicyHandler : PathHandler, IContentReaderHandler
{
    private readonly RolePoliciesHandler _parentHandler;

        public RolePolicyHandler(ItemPath path, IPathHandlerContext context, IAmazonIdentityManagementService iam, IItemAncestor<RoleItem> role) : base(path, context)
    {
        _parentHandler = new RolePoliciesHandler(path.Parent, context, iam, role);
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

    public IStreamContentReader GetContentReader()
    {
        var item = GetItem() as EntityPolicyItem;
        if (item == null)
        {
            throw new InvalidOperationException("Item does not exist");
        }
        return new StreamContentReader(new MemoryStream(Encoding.UTF8.GetBytes(item.RawDocument)));
    }
}