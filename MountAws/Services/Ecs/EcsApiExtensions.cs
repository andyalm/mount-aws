using Amazon.ECS;
using Amazon.ECS.Model;
using MountAws.Services.Ecs;
using static MountAws.PagingHelper;
using Task = Amazon.ECS.Model.Task;

namespace MountAws.Api.AwsSdk.Ecs;

public static class EcsApiExtensions
{
    public static IEnumerable<ContainerInstance> QueryContainerInstances(this IAmazonECS ecs, string clusterName, string? filter = null)
    {
        if (filter?.StartsWith("i-") == true)
        {
            filter = $"ec2InstanceId=~{filter}";
        }
        
        var containerInstanceArns = ecs.ListContainerInstances(clusterName, filter);

        return containerInstanceArns.Chunk(100).SelectMany(containerInstanceArnChunk => 
            ecs.DescribeContainerInstances(clusterName, containerInstanceArnChunk, ContainerInstanceHandler.Include));
    }
    public static IEnumerable<string> ListClusters(this IAmazonECS ecs)
    {
        return Paginate(nextToken =>
        {
            var response = ecs.ListClustersAsync(new ListClustersRequest
            {
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return (response.ClusterArns, response.NextToken);
        });
    }

    public static IEnumerable<Cluster> DescribeClusters(this IAmazonECS ecs, IEnumerable<string> clusters, IEnumerable<string>? include = null)
    {
        return clusters.Chunk(100).SelectMany(clustersPage => ecs.DescribeClustersAsync(new DescribeClustersRequest
        {
            Clusters = clustersPage.ToList(),
            Include = include?.ToList()
        }).GetAwaiter().GetResult().Clusters);
    }

    public static Cluster DescribeCluster(this IAmazonECS ecs, string cluster, IEnumerable<string>? include = null)
    {
        var response = ecs.DescribeClustersAsync(new DescribeClustersRequest
        {
            Clusters = new List<string> { cluster },
            Include = include?.ToList()
        }).GetAwaiter().GetResult();

        var clusterObject = response.Clusters.FirstOrDefault();
        if (clusterObject == null)
        {
            throw new ClusterNotFoundException(cluster);
        }

        return clusterObject;
    }

    public static IEnumerable<string> ListContainerInstances(this IAmazonECS ecs, string cluster, string? filter = null)
    {
        return Paginate(nextToken =>
        {
            var response = ecs.ListContainerInstancesAsync(new ListContainerInstancesRequest
            {
                Cluster = cluster,
                Filter = filter,
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return (response.ContainerInstanceArns, response.NextToken);
        });
    }

    public static ContainerInstance DescribeContainerInstance(this IAmazonECS ecs, string cluster, string containerInstanceId, IEnumerable<string>? include = null)
    {
        var request = new DescribeContainerInstancesRequest
        {
            Cluster = cluster,
            ContainerInstances = new List<string> { containerInstanceId },
            Include = include?.ToList()
        };

        var containerInstance = ecs.DescribeContainerInstancesAsync(request)
            .GetAwaiter()
            .GetResult()
            .ContainerInstances
            .FirstOrDefault();

        if (containerInstance == null)
        {
            throw new ContainerInstanceNotFoundException(cluster, containerInstanceId);
        }

        return containerInstance;
    }

    public static IEnumerable<ContainerInstance> DescribeContainerInstances(this IAmazonECS ecs, string cluster, IEnumerable<string> containerInstanceIds, IEnumerable<string>? include = null)
    {
        return ecs.DescribeContainerInstancesAsync(new DescribeContainerInstancesRequest
        {
            Cluster = cluster,
            ContainerInstances = containerInstanceIds.ToList(),
            Include = include?.ToList()
        }).GetAwaiter().GetResult().ContainerInstances;
    }

    public static IEnumerable<string> ListServices(this IAmazonECS ecs, string cluster)
    {
        return Paginate(nextToken =>
        {
            var response = ecs.ListServicesAsync(new ListServicesRequest
            {
                Cluster = cluster,
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return (response.ServiceArns, response.NextToken);
        });
    }

    public static IEnumerable<Service> DescribeServices(this IAmazonECS ecs, string cluster, IEnumerable<string> serviceIds, IEnumerable<string>? include = null)
    {
        return ecs.DescribeServicesAsync(new DescribeServicesRequest
        {
            Cluster = cluster,
            Services = serviceIds.ToList(),
            Include = include?.ToList()
        }).GetAwaiter().GetResult().Services;
    }

    public static IEnumerable<string> ListTasksByContainerInstance(this IAmazonECS ecs, string cluster, string containerInstanceId)
    {
        return Paginate(nextToken =>
        {
            var response = ecs.ListTasksAsync(new ListTasksRequest
            {
                Cluster = cluster,
                ContainerInstance = containerInstanceId,
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return (response.TaskArns, response.NextToken);
        });
    }
    
    public static IEnumerable<string> ListTasksByService(this IAmazonECS ecs, string cluster, string serviceName)
    {
        return Paginate(nextToken =>
        {
            var response = ecs.ListTasksAsync(new ListTasksRequest
            {
                Cluster = cluster,
                ServiceName = serviceName,
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return (response.TaskArns, response.NextToken);
        });
    }

    public static IEnumerable<Task> DescribeTasks(this IAmazonECS ecs, string cluster, IEnumerable<string> taskIds, IEnumerable<string>? include = null)
    {
        return ecs.DescribeTasksAsync(new DescribeTasksRequest
        {
            Cluster = cluster,
            Tasks = taskIds.ToList(),
            Include = include?.ToList()
        }).GetAwaiter().GetResult().Tasks;
    }

    public static void StopTask(this IAmazonECS ecs, string cluster, string taskId, string? reason = null)
    {
        ecs.StopTaskAsync(new StopTaskRequest
        {
            Cluster = cluster,
            Task = taskId,
            Reason = reason
        }).GetAwaiter().GetResult();
    }

    public static void DeleteService(this IAmazonECS ecs, string cluster, string serviceName, bool force)
    {
        ecs.DeleteServiceAsync(new DeleteServiceRequest
        {
            Cluster = cluster,
            Service = serviceName,
            Force = force
        }).GetAwaiter().GetResult();
    }

    public static IEnumerable<string> ListTaskFamilies(this IAmazonECS ecs, string? familyPrefix, int? maxResults = null)
    {
        return Paginate(nextToken =>
        {
            var request = new ListTaskDefinitionFamiliesRequest
            {
                NextToken = nextToken,
                FamilyPrefix = familyPrefix,
                Status = TaskDefinitionFamilyStatus.ACTIVE
            };
            if (maxResults != null)
            {
                request.MaxResults = maxResults.Value;
            }

            var response = ecs.ListTaskDefinitionFamiliesAsync(request).GetAwaiter().GetResult();

            return (response.Families.ToArray(), response.NextToken);
        });
    }

    public static IEnumerable<string> ListTaskDefinitionsByFamily(this IAmazonECS ecs, string family, bool isActive = true)
    {
        return Paginate(nextToken =>
        {
            var response = ecs.ListTaskDefinitionsAsync(new ListTaskDefinitionsRequest
            {
                FamilyPrefix = family,
                Sort = SortOrder.ASC,
                Status = isActive ? TaskDefinitionStatus.ACTIVE : TaskDefinitionStatus.INACTIVE,
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return (response.TaskDefinitionArns, response.NextToken);
        });
    }

    public static (TaskDefinition TaskDefinition, Tag[] Tags) DescribeTaskDefinition(this IAmazonECS ecs, string taskDefinition)
    {
        var response = ecs.DescribeTaskDefinitionAsync(new DescribeTaskDefinitionRequest
        {
            TaskDefinition = taskDefinition,
            Include = new List<string> { "TAGS" }
        }).GetAwaiter().GetResult();

        return (response.TaskDefinition, response.Tags.ToArray());
    }
}