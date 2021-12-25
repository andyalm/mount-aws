using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Elbv2;

public class Elbv2RootHandler : PathHandler
{
    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "elbv2",
            "Navigate load balancers and associated objects");
    }
    
    public Elbv2RootHandler(string path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override bool ExistsImpl()
    {
        return true;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return LoadBalancersHandler.CreateItem(Path);
    }
}