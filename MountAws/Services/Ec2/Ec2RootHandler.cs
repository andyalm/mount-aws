using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Ec2;

public class Ec2RootHandler : PathHandler
{
    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "ec2", "Navigate EC2 instances and related objects");
    }
    
    public Ec2RootHandler(ItemPath path, IPathHandlerContext context) : base(path, context)
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
        yield return AutoScalingGroupsHandler.CreateItem(Path);
        yield return ImagesHandler.CreateItem(Path);
        yield return InstancesHandler.CreateItem(Path);
        yield return SecurityGroupsHandler.CreateItem(Path);
        yield return VpcsHandler.CreateItem(Path);
    }
}