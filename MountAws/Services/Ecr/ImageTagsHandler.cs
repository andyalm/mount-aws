using System.Management.Automation;
using MountAnything;
using MountAws.Api.Ecr;
using MountAws.Services.Core;

using static MountAws.PagingHelper;

namespace MountAws.Services.Ecr;

public class ImageTagsHandler : PathHandler
{
    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "image-tags",
            "Navigate the docker image tags for this repository");
    }
    
    private readonly IEcrApi _ecr;
    private readonly RepositoryPath _repositoryPath;

    public ImageTagsHandler(string path, IPathHandlerContext context, IEcrApi ecr, RepositoryPath repositoryPath) : base(path, context)
    {
        _ecr = ecr;
        _repositoryPath = repositoryPath;
    }

    protected override Item? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        var repositoryHandler = new RepositoryHandler(ParentPath, Context, _ecr, _repositoryPath);
        var repositoryItem = repositoryHandler.GetItem() as RepositoryItem;
        if (repositoryItem?.ItemType != EcrItemTypes.Repository)
        {
            return Enumerable.Empty<Item>();
        }
        
        return GetWithPaging(nextToken =>
        {
            var response = _ecr.ListTaggedImages(_repositoryPath.Value, nextToken);

            return new PaginatedResponse<PSObject>
            {
                PageOfResults = response.ImageIds,
                NextToken = response.NextToken
            };
        }).Select(i => new ImageTagItem(Path, repositoryItem.UnderlyingObject, i));
    }
}