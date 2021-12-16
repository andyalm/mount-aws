using Amazon.ECS;
using Amazon.ECS.Model;

using static MountAws.PagingHelper;

namespace MountAws.Services.ECS;

public static class ECSClientExtensions
{
    public static IEnumerable<ContainerInstance> QueryContainerInstances(this IAmazonECS ecs, string clusterName, string? filter = null)
    {
        if (filter?.StartsWith("i-") == true)
        {
            filter = $"ec2InstanceId=~{filter}";
        }
        
        var containerInstanceArns = GetWithPaging(nextToken =>
        {
            var response = ecs.ListContainerInstancesAsync(new ListContainerInstancesRequest
            {
                Cluster = clusterName,
                Filter = filter,
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return new PaginatedResponse<string>
            {
                PageOfResults = response.ContainerInstanceArns.ToArray(),
                NextToken = nextToken
            };
        });

        return containerInstanceArns.Chunk(100).SelectMany(containerInstanceArnChunk =>
        {
            return ecs.DescribeContainerInstancesAsync(new DescribeContainerInstancesRequest
            {
                Cluster = clusterName,
                ContainerInstances = containerInstanceArnChunk.ToList(),
                Include = ContainerInstanceHandler.Include
            }).GetAwaiter().GetResult().ContainerInstances;
        });
    }
}