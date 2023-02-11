using Amazon.RDS;
using Amazon.RDS.Model;

using static MountAws.PagingHelper;

namespace MountAws.Services.Rds;

public static class ApiExtensions
{
    public static IEnumerable<DBInstance> DescribeDBInstances(this IAmazonRDS rds, Filter filter)
    {
        return rds.DescribeDBInstances(new[] { filter });
    }
    
    public static IEnumerable<DBInstance> DescribeDBInstances(this IAmazonRDS rds, IEnumerable<Filter>? filters = null)
    {
        return Paginate(nextToken =>
        {
            var response = rds.DescribeDBInstancesAsync(new DescribeDBInstancesRequest
                {
                    Filters = filters?.ToList(),
                    Marker = nextToken
                })
                .GetAwaiter()
                .GetResult();

            return (response.DBInstances, response.Marker);
        });
    }
    
    public static DBInstance? DescribeDBInstance(this IAmazonRDS rds, string dbInstanceIdentifier)
    {
        try
        {
            return rds.DescribeDBInstancesAsync(new DescribeDBInstancesRequest
                {
                    DBInstanceIdentifier = dbInstanceIdentifier
                })
                .GetAwaiter()
                .GetResult()
                .DBInstances
                .Single();
        }
        catch (DBInstanceNotFoundException)
        {
            return null;
        }
    }
    
    public static IEnumerable<DBCluster> DescribeDBClusters(this IAmazonRDS rds, IEnumerable<Filter>? filters = null)
    {
        return Paginate(nextToken =>
        {
            var response = rds.DescribeDBClustersAsync(new DescribeDBClustersRequest
                {
                    Filters = filters?.ToList(),
                    Marker = nextToken,
                    IncludeShared = true
                })
                .GetAwaiter()
                .GetResult();

            return (response.DBClusters, response.Marker);
        });
    }
    
    public static DBCluster? DescribeDBCluster(this IAmazonRDS rds, string dbClusterIdentifier)
    {
        try
        {
            return rds.DescribeDBClustersAsync(new DescribeDBClustersRequest
                {
                    DBClusterIdentifier = dbClusterIdentifier
                })
                .GetAwaiter()
                .GetResult()
                .DBClusters
                .Single();
        }
        catch (DBClusterNotFoundException)
        {
            return null;
        }
    }
}