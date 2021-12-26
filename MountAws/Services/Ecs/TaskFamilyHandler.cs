using MountAnything;
using MountAws.Api.Ecs;

using static MountAws.PagingHelper;

namespace MountAws.Services.Ecs;

public class TaskFamilyHandler : PathHandler
{
    private readonly IEcsApi _ecs;

    public TaskFamilyHandler(string path, IPathHandlerContext context, IEcsApi ecs) : base(path, context)
    {
        _ecs = ecs;
    }

    protected override IItem? GetItemImpl()
    {
        //TODO: verify existence
        return new TaskFamilyItem(ParentPath, ItemName);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var inactiveTaskDefinitions = Enumerable.Empty<TaskDefinitionItem>();
        if (Context.Force)
        {
            inactiveTaskDefinitions = GetTaskDefinitions(false);
        }

        var activeTaskDefinitions = GetTaskDefinitions(true);

        return inactiveTaskDefinitions.Concat(activeTaskDefinitions);
    }

    private IEnumerable<TaskDefinitionItem> GetTaskDefinitions(bool isActive)
    {
        return GetWithPaging(nextToken =>
        {
            var response = _ecs.ListTaskDefinitionsByFamily(ItemName, isActive, nextToken);

            return new PaginatedResponse<string>
            {
                PageOfResults = response.TaskDefinitionArns,
                NextToken = response.NextToken
            };
        }).Select(arn => new TaskDefinitionItem(Path, arn));
    }
}