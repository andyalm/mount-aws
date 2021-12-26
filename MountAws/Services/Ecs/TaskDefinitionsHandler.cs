using System.Management.Automation;
using MountAnything;
using MountAws.Api.Ecs;
using MountAws.Services.Core;

using static MountAws.PagingHelper;

namespace MountAws.Services.Ecs;

public class TaskDefinitionsHandler : PathHandler
{
    private readonly IEcsApi _ecs;

    public static IItem CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "task-definitions",
            "Navigate the task families in the current account and region");
    }
    
    public TaskDefinitionsHandler(string path, IPathHandlerContext context, IEcsApi ecs) : base(path, context)
    {
        _ecs = ecs;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        int pageSize = 100;
        return GetWithPaging(nextToken =>
            {
                var response = _ecs.ListTaskFamilies(nextToken, maxResults:pageSize);

                return new PaginatedResponse<string>
                {
                    PageOfResults = response.Families,
                    NextToken = response.NextToken
                };
            }, 10)
            .Select(t => new TaskFamilyItem(Path, t))
            .WarnIfMoreItemsThan(1000, Context, "Not all task families were returned because there are too many. Use the -filter argument to scope the results");
    }

    public override IEnumerable<IItem> GetChildItems(string filter)
    {
        return GetWithPaging(nextToken =>
        {
            var response = _ecs.ListTaskFamilies(nextToken, filter.Replace("*", ""));

            return new PaginatedResponse<string>
            {
                PageOfResults = response.Families,
                NextToken = response.NextToken
            };
        }).Select(t => new TaskFamilyItem(Path, t));
    }
}