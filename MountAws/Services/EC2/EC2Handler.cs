using Amazon.EC2;
using MountAws.Services.Core;

namespace MountAws.Services.EC2;

public class EC2Handler : PathHandler
{
    public static AwsItem CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "ec2", "Navigate EC2 instances and related objects");
    }
    
    public EC2Handler(string path, IPathHandlerContext context) : base(path, context)
    {
        
    }

    protected override bool ExistsImpl()
    {
        return true;
    }

    protected override AwsItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<AwsItem> GetChildItemsImpl()
    {
        yield return EC2InstancesHandler.CreateItem(Path);
    }
}