using System.Management.Automation;
using Amazon.ECS;
using Amazon.ECS.Model;
using MountAnything;
using MountAws.Api.Ecs;
using ClusterNotFoundException = Amazon.ECS.Model.ClusterNotFoundException;
using ListClustersResponse = MountAws.Api.Ecs.ListClustersResponse;
using ListContainerInstancesResponse = MountAws.Api.Ecs.ListContainerInstancesResponse;
using ListServicesResponse = MountAws.Api.Ecs.ListServicesResponse;
using ListTasksResponse = MountAws.Api.Ecs.ListTasksResponse;

namespace MountAws.Api.AwsSdk.Ecs;

public class AwsSdkEcsApi : IEcsApi
{
    private readonly IAmazonECS _ecs;

    public AwsSdkEcsApi(IAmazonECS ecs)
    {
        _ecs = ecs;
    }

    public ListClustersResponse ListClusters(string? nextToken)
    {
        var response = _ecs.ListClustersAsync(new ListClustersRequest
        {
            NextToken = nextToken
        }).GetAwaiter().GetResult();

        return new ListClustersResponse(response.ClusterArns, response.NextToken);
    }

    public IEnumerable<PSObject> DescribeClusters(IEnumerable<string> clusters, IEnumerable<string>? include = null)
    {
        return _ecs.DescribeClustersAsync(new DescribeClustersRequest
        {
            Clusters = clusters.ToList(),
            Include = include?.ToList()
        }).GetAwaiter().GetResult().Clusters.ToPSObjects();
    }

    public PSObject DescribeCluster(string cluster, IEnumerable<string>? include = null)
    {
        try
        {
            var response = _ecs.DescribeClustersAsync(new DescribeClustersRequest
            {
                Clusters = new List<string> { cluster },
                Include = include?.ToList()
            }).GetAwaiter().GetResult();

            var clusterObject = response.Clusters.FirstOrDefault()?.ToPSObject();
            if (clusterObject == null)
            {
                throw new Api.Ecs.ClusterNotFoundException(cluster);
            }

            return clusterObject;
        }
        catch (ClusterNotFoundException)
        {
            throw new Api.Ecs.ClusterNotFoundException(cluster);
        }
        
    }

    public ListContainerInstancesResponse ListContainerInstances(string cluster, string? filter = null, string? nextToken = null)
    {
        var response = _ecs.ListContainerInstancesAsync(new ListContainerInstancesRequest
        {
            Cluster = cluster,
            Filter = filter,
            NextToken = nextToken
        }).GetAwaiter().GetResult();

        return new ListContainerInstancesResponse(response.ContainerInstanceArns, response.NextToken);
    }

    public PSObject DescribeContainerInstance(string cluster, string containerInstanceId, IEnumerable<string>? include = null)
    {
        var request = new DescribeContainerInstancesRequest
        {
            Cluster = cluster,
            ContainerInstances = new List<string> { containerInstanceId },
            Include = include?.ToList()
        };

        var containerInstance = _ecs.DescribeContainerInstancesAsync(request)
            .GetAwaiter()
            .GetResult()
            .ContainerInstances
            .FirstOrDefault();

        if (containerInstance == null)
        {
            throw new ContainerInstanceNotFoundException(cluster, containerInstanceId);
        }

        return containerInstance.ToPSObject();
    }

    public IEnumerable<PSObject> DescribeContainerInstances(string cluster, IEnumerable<string> containerInstanceIds, IEnumerable<string>? include = null)
    {
        return _ecs.DescribeContainerInstancesAsync(new DescribeContainerInstancesRequest
        {
            Cluster = cluster,
            ContainerInstances = containerInstanceIds.ToList(),
            Include = include?.ToList()
        }).GetAwaiter().GetResult().ContainerInstances.ToPSObjects();
    }

    public ListServicesResponse ListServices(string cluster, string? nextToken = null)
    {
        var response = _ecs.ListServicesAsync(new ListServicesRequest
        {
            Cluster = cluster,
            NextToken = nextToken
        }).GetAwaiter().GetResult();

        return new ListServicesResponse(response.ServiceArns, response.NextToken);
    }

    public IEnumerable<PSObject> DescribeServices(string cluster, IEnumerable<string> serviceIds, IEnumerable<string>? include = null)
    {
        return _ecs.DescribeServicesAsync(new DescribeServicesRequest
        {
            Cluster = cluster,
            Services = serviceIds.ToList(),
            Include = include?.ToList()
        }).GetAwaiter().GetResult().Services.ToPSObjects();
    }

    public ListTasksResponse ListTasksByContainerInstance(string cluster, string containerInstanceId, string? nextToken)
    {
        var response = _ecs.ListTasksAsync(new ListTasksRequest
        {
            Cluster = cluster,
            ContainerInstance = containerInstanceId,
            NextToken = nextToken
        }).GetAwaiter().GetResult();

        return new ListTasksResponse(response.TaskArns, response.NextToken);
    }
    
    public ListTasksResponse ListTasksByService(string cluster, string serviceName, string? nextToken)
    {
        var response = _ecs.ListTasksAsync(new ListTasksRequest
        {
            Cluster = cluster,
            ServiceName = serviceName,
            NextToken = nextToken
        }).GetAwaiter().GetResult();

        return new ListTasksResponse(response.TaskArns, response.NextToken);
    }

    public IEnumerable<PSObject> DescribeTasks(string cluster, IEnumerable<string> taskIds, IEnumerable<string>? include = null)
    {
        return _ecs.DescribeTasksAsync(new DescribeTasksRequest
        {
            Cluster = cluster,
            Tasks = taskIds.ToList(),
            Include = include?.ToList()
        }).GetAwaiter().GetResult().Tasks.ToPSObjects();
    }

    public void StopTask(string cluster, string taskId, string? reason = null)
    {
        _ecs.StopTaskAsync(new StopTaskRequest
        {
            Cluster = cluster,
            Task = taskId,
            Reason = reason
        }).GetAwaiter().GetResult();
    }

    public void DeleteService(string cluster, string serviceName, bool force)
    {
        _ecs.DeleteServiceAsync(new DeleteServiceRequest
        {
            Cluster = cluster,
            Service = serviceName,
            Force = force
        }).GetAwaiter().GetResult();
    }
}