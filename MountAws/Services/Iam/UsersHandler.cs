using Amazon.IdentityManagement;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Iam;

public class UsersHandler : PathHandler
{
    private readonly UserNavigator _navigator;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "users",
            "Navigate iam users as a virtual filesystem");
    }
    
    public UsersHandler(ItemPath path, IPathHandlerContext context, UserNavigator navigator) : base(path, context)
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