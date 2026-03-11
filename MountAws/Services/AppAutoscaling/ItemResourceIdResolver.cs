using MountAnything;

namespace MountAws.Services.AppAutoscaling;

public class ItemResourceIdResolver<TItem>(IItemAncestor<TItem> itemAncestor, Func<TItem,string> resourceIdAccessor)
    : IResourceIdResolver where TItem : IItem
{
    public string ResourceId => resourceIdAccessor(itemAncestor.Item);
}