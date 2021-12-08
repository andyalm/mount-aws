namespace MountAws.Services.EC2;

public class EC2Handler : PathHandler
{
    public EC2Handler(string path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override bool ExistsImpl()
    {
        throw new NotImplementedException();
    }

    protected override AwsItem? GetItemImpl()
    {
        throw new NotImplementedException();
    }

    protected override IEnumerable<AwsItem> GetChildItemsImpl()
    {
        throw new NotImplementedException();
    }
}