using MountAnything;
using MountAws.Api.Ecr;
using MountAws.Services.Core;

using static MountAws.PagingHelper;

namespace MountAws.Services.Ecr;

public class EcrRootHandler : PathHandler
{
    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "ecr",
            "Navigate the ECR repositories");
    }
    
    private readonly IEcrApi _ecr;

    public EcrRootHandler(string path, IPathHandlerContext context, IEcrApi ecr) : base(path, context)
    {
        _ecr = ecr;
    }

    protected override Item? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        return _ecr.GetChildRepositories(Path);
    }
}