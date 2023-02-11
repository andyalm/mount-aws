using Amazon.RDS;
using Amazon.RDS.Model;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Rds;

public class InstancesHandler : PathHandler, IGetChildItemParameters<InstancesParameters>
{
    private readonly IAmazonRDS _rds;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "instances",
            "Navigate RDS DB instances in the current account and region");
    }
    
    public InstancesHandler(ItemPath path, IPathHandlerContext context, IAmazonRDS rds) : base(path, context)
    {
        _rds = rds;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _rds.DescribeDBInstances(GetFilters())
            .Select(db => new InstanceItem(Path, db));
    }

    public override IEnumerable<IItem> GetChildItems(string filter)
    {
        if (filter.Contains("="))
        {
            var filters = GetFilters().Concat(new[] { ParseKeyValueFilter(filter) });
        
            return _rds.DescribeDBInstances(filters)
                .Select(db => new InstanceItem(Path, db));
        }

        return base.GetChildItems(filter);
    }

    private Filter ParseKeyValueFilter(string filter)
    {
        var parts = filter.Split("=");
        if (parts.Length != 2)
            throw new ArgumentException($"Filter expression '{filter}' not supported");
        
        return new Filter
        {
            Name = parts[0],
            Values = new List<string>{parts[1]}
        };
    }

    private IEnumerable<Filter> GetFilters()
    {
        if (GetChildItemParameters?.Engine != null)
        {
            yield return new Filter
            {
                Name = "engine",
                Values = new List<string>{GetChildItemParameters.Engine}
            };
        }
    }

    protected override bool CacheChildren => GetChildItemParameters?.Engine == null;

    public InstancesParameters? GetChildItemParameters { get; set; }
}