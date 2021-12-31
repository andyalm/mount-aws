using Amazon.ECR;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Ecr;

public class EcrRootHandler : PathHandler
{
    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "ecr",
            "Navigate the ECR repositories");
    }
    
    private readonly IAmazonECR _ecr;

    public EcrRootHandler(ItemPath path, IPathHandlerContext context, IAmazonECR ecr) : base(path, context)
    {
        _ecr = ecr;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _ecr.GetChildRepositories(Path);
    }
}