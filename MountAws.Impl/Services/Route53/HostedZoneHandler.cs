using System.Management.Automation;
using MountAnything;
using MountAws.Api.Route53;

using static MountAws.PagingHelper;

namespace MountAws.Services.Route53;

public class HostedZoneHandler : PathHandler
{
    private readonly IRoute53Api _route53;

    public HostedZoneHandler(string path, IPathHandlerContext context, IRoute53Api route53) : base(path, context)
    {
        _route53 = route53;
    }

    protected override IItem? GetItemImpl()
    {
        try
        {
            var hostedZone = _route53.GetHostedZone(ItemName);
            return new HostedZoneItem(ParentPath, hostedZone);
        }
        catch (HostedZoneNotFoundException)
        {
            return null;
        }
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return GetWithPaging(nextToken =>
        {
            var response = _route53.ListResourceRecordSets(ItemName, nextToken);

            return new PaginatedResponse<PSObject>
            {
                PageOfResults = response.Records.ToArray(),
                NextToken = response.NextToken
            };
        }).Select(r => new ResourceRecordItem(Path, r));
    }
}