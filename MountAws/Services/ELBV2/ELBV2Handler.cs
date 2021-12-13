using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.ELBV2;

public class ELBV2Handler : PathHandler
{
    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "elbv2",
            "Navigate load balancers and associated objects");
    }
    
    public ELBV2Handler(string path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override bool ExistsImpl()
    {
        return true;
    }

    protected override Item? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        yield return LoadBalancersHandler.CreateItem(Path);
    }
}