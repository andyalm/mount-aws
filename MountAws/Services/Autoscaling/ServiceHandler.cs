using Amazon.ApplicationAutoScaling;
using MountAnything;

namespace MountAws.Services.Autoscaling;

public class ServiceHandler : PathHandler
{
    public ServiceHandler(ItemPath path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override IItem? GetItemImpl()
    {
        var serviceNamespace = new ServiceNamespace(Path.Name);
        
        return new ServiceItem(ParentPath, serviceNamespace);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return ScalableTargetsHandler.CreateItem(Path);
    }
}