using Amazon.ECR;
using Amazon.ECR.Model;
using MountAnything;
using MountAws.Services.Core;

using static MountAws.PagingHelper;

namespace MountAws.Services.ECR;

public class ImageTagsHandler : PathHandler
{
    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "image-tags",
            "Navigate the docker image tags for this repository");
    }
    
    private readonly IAmazonECR _ecr;
    private readonly RepositoryPath _repositoryPath;

    public ImageTagsHandler(string path, IPathHandlerContext context, IAmazonECR ecr, RepositoryPath repositoryPath) : base(path, context)
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
        if (repositoryItem?.Repository == null)
        {
            return Enumerable.Empty<Item>();
        }
        
        return GetWithPaging(nextToken =>
        {
            var response = _ecr.ListImagesAsync(new ListImagesRequest
            {
                RepositoryName = _repositoryPath.Value,
                NextToken = nextToken,
                Filter = new ListImagesFilter
                {
                    TagStatus = TagStatus.TAGGED
                }
            }).GetAwaiter().GetResult();

            return new PaginatedResponse<ImageIdentifier>
            {
                PageOfResults = response.ImageIds.ToArray(),
                NextToken = response.NextToken
            };
        }).Select(i => new ImageTagItem(Path, repositoryItem.Repository, i));
    }
}