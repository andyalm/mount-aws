using Amazon.IdentityManagement;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Iam;

public class RolesHandler : PathHandler
{
    private readonly IAmazonIdentityManagementService _iam;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "roles",
            "Navigate iam roles as a virtual filesystem");
    }
    
    public RolesHandler(ItemPath path, IPathHandlerContext context, IAmazonIdentityManagementService iam) : base(path, context)
    {
        _iam = iam;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _iam.ListChildRoleItems(Path);
    }
}