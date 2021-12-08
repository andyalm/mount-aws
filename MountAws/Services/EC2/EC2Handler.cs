using Amazon.EC2;

namespace MountAws.Services.EC2;

public class EC2Handler : PathHandler
{
    public EC2Handler(string path, IPathHandlerContext context) : base(path, context)
    {
        
    }

    protected override bool ExistsImpl()
    {
        return true;
    }

    protected override AwsItem? GetItemImpl()
    {
        return new GenericContainerItem(ParentPath, ItemName);
    }

    protected override IEnumerable<AwsItem> GetChildItemsImpl()
    {
        yield return new GenericContainerItem(Path, "instances");
    }
}