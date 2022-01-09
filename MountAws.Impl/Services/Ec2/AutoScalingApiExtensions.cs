using Amazon.AutoScaling;
using Amazon.AutoScaling.Model;
using static MountAws.PagingHelper;

namespace MountAws.Services.Ec2;

public static class AutoScalingApiExtensions
{
    public static IEnumerable<AutoScalingGroup> DescribeAutoScalingGroups(this IAmazonAutoScaling autoScaling, string? filter = null)
    {
        var filters = ParseFilters(filter);
        
        return Paginate(nextToken =>
        {
            var response = autoScaling.DescribeAutoScalingGroupsAsync(new DescribeAutoScalingGroupsRequest
            {
                Filters = filters,
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return (response.AutoScalingGroups, response.NextToken);
        });
    }
    
    public static AutoScalingGroup DescribeAutoScalingGroup(this IAmazonAutoScaling autoScaling, string name)
    {
        var response = autoScaling.DescribeAutoScalingGroupsAsync(new DescribeAutoScalingGroupsRequest
        {
            AutoScalingGroupNames = new List<string>{name}
        }).GetAwaiter().GetResult();

        return response.AutoScalingGroups.Single();
    }

    private static List<Filter>? ParseFilters(string? filterString)
    {
        if (filterString == null)
        {
            return null;
        }
        
        var filters = new List<Filter>();
        foreach (var filter in filterString.Split(","))
        {
            if (filter.Contains('='))
            {
                var parts = filter.Split("=");
                filters.Add(new Filter
                {
                    Name = parts[0],
                    Values = new List<string>{parts[1]}
                });
            }
            else
            {
                filters.Add(new Filter
                {
                    Name = "tag:Name",
                    Values = new List<string>{filter}
                });
            }
        }

        return filters;
    }
}