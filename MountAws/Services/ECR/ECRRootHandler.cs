using Amazon.ECR;
using Amazon.ECR.Model;
using MountAnything;
using MountAws.Services.Core;

using static MountAws.PagingHelper;

namespace MountAws.Services.ECR;

public class ECRRootHandler : PathHandler
{
    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "ecr",
            "Navigate the ECR repositories");
    }
    
    private readonly IAmazonECR _ecr;

    public ECRRootHandler(string path, IPathHandlerContext context, IAmazonECR ecr) : base(path, context)
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