using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Autoscaling;

public class ServicesHandler : PathHandler
{
    
    
    public ServicesHandler(ItemPath path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override IItem? GetItemImpl()
    {
        throw new NotImplementedException();
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        throw new NotImplementedException();
    }
}