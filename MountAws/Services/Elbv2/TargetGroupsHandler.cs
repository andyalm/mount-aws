using System.Management.Automation;
using MountAnything;
using MountAws.Api.Elbv2;
using MountAws.Services.Core;

using static MountAws.PagingHelper;

namespace MountAws.Services.Elbv2;

public class TargetGroupsHandler : PathHandler
{
    public static IItem CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "target-groups",
            "Navigate the target groups in the current account and region");
    }
    
    private readonly IElbv2Api _elbv2;

    public TargetGroupsHandler(string path, IPathHandlerContext context, IElbv2Api elbv2) : base(path, context)
    {
        _elbv2 = elbv2;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return GetWithPaging(nextToken =>
        {
            var response = _elbv2.DescribeTargetGroups(nextToken);

            return new PaginatedResponse<PSObject> {
                PageOfResults = response.TargetGroups.ToArray(),
                NextToken = response.NextToken
            };
        }).Select(t => new TargetGroupItem(Path, t))
            .OrderBy(t => t.ItemName);
    }
}