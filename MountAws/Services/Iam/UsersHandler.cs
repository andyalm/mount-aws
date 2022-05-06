using Amazon.IdentityManagement;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Iam;

public class UsersHandler : PathHandler
{
    private readonly IAmazonIdentityManagementService _iam;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "users",
            "Navigate iam users as a virtual filesystem");
    }
    
    public UsersHandler(ItemPath path, IPathHandlerContext context, IAmazonIdentityManagementService iam) : base(path, context)
    {
        _iam = iam;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _iam.ListChildUserItems(Path);
    }
}