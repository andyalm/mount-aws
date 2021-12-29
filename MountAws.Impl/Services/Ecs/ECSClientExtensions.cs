using System.Management.Automation;
using MountAws.Api.Ecs;
using static MountAws.PagingHelper;

namespace MountAws.Services.Ecs;

public static class ECSClientExtensions
{
    public static IEnumerable<PSObject> QueryContainerInstances(this IEcsApi ecs, string clusterName, string? filter = null)
    {
        if (filter?.StartsWith("i-") == true)
        {
            filter = $"ec2InstanceId=~{filter}";
        }
        
        var containerInstanceArns = GetWithPaging(nextToken =>
        {
            var response = ecs.ListContainerInstances(clusterName, filter, nextToken);

            return new PaginatedResponse<string>
            {
                PageOfResults = response.ContainerInstanceArns.ToArray(),
                NextToken = nextToken
            };
        });

        return containerInstanceArns.Chunk(100).SelectMany(containerInstanceArnChunk => 
            ecs.DescribeContainerInstances(clusterName, containerInstanceArnChunk, ContainerInstanceHandler.Include));
    }
}