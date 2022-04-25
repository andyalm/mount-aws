using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Iam;

public class IamRootHandler : PathHandler
{
    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "iam",
            "Navigate iam resources as a virtual filesystem");
    }
    
    public IamRootHandler(ItemPath path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return PoliciesHandler.CreateItem(Path);
        yield return RolesHandler.CreateItem(Path);
        yield return UsersHandler.CreateItem(Path);
    }
}