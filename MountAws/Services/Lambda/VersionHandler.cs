using Amazon.Lambda;
using Amazon.Lambda.Model;
using MountAnything;

namespace MountAws.Services.Lambda;

public class VersionHandler : PathHandler
{
    private readonly IAmazonLambda _lambda;
    private readonly CurrentFunction _currentFunction;

    public VersionHandler(ItemPath path, IPathHandlerContext context,
        IAmazonLambda lambda, CurrentFunction currentFunction)
        : base(path, context)
    {
        _lambda = lambda;
        _currentFunction = currentFunction;
    }

    protected override IItem? GetItemImpl()
    {
        try
        {
            var version = _lambda.GetFunction($"{_currentFunction.Name}:{ItemName}");
            return new VersionItem(ParentPath, version);
        }
        catch (ResourceNotFoundException)
        {
            return null;
        }
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}
