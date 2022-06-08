using Amazon.IdentityManagement;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Iam;

public class RolesHandler : PathHandler
{
    private readonly RoleNavigator _navigator;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "roles",
            "Navigate iam roles as a virtual filesystem");
    }
    
    public RolesHandler(ItemPath path, IPathHandlerContext context, RoleNavigator navigator) : base(path, context)
    {
        _navigator = navigator;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _navigator.ListChildItems(Path);
    }
}