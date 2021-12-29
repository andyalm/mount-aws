using System.Management.Automation;
using MountAnything;
using MountAws.Api.Route53;
using MountAws.Services.Core;

using static MountAws.PagingHelper;

namespace MountAws.Services.Route53;

public class HostedZonesHandler : PathHandler
{
    private readonly IRoute53Api _route53;

    public static IItem CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "hosted-zones",
            "Contains hosted zones and child dns records");
    }
    
    public HostedZonesHandler(string path, IPathHandlerContext context, IRoute53Api route53) : base(path, context)
    {
        _route53 = route53;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return GetWithPaging(nextToken =>
        {
            var response = _route53.ListHostedZones(nextToken);

            return new PaginatedResponse<PSObject>
            {
                PageOfResults = response.HostedZones.ToArray(),
                NextToken = response.NextToken
            };
        }).Select(z => new HostedZoneItem(Path, z))
            .OrderBy(i => i.Name);
    }
}