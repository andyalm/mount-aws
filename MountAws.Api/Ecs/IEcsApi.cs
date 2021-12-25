using System.Management.Automation;

namespace MountAws.Api.Ecs;

public interface IEcsApi
{
    ListClustersResponse ListClusters(string? nextToken);
    IEnumerable<PSObject> DescribeClusters(IEnumerable<string> clusters, IEnumerable<string>? include = null);

    PSObject DescribeCluster(string cluster, IEnumerable<string>? include = null);

    ListContainerInstancesResponse ListContainerInstances(string cluster, string? filter = null, string? nextToken = null);
    PSObject DescribeContainerInstance(string cluster, string containerInstanceArn,
        IEnumerable<string>? include = null);

    IEnumerable<PSObject> DescribeContainerInstances(string cluster, IEnumerable<string> containerInstanceIds,
        IEnumerable<string>? include = null);

    ListServicesResponse ListServices(string cluster, string? nextToken = null);

    IEnumerable<PSObject> DescribeServices(string cluster, IEnumerable<string> serviceIds, IEnumerable<string>? include = null);

    ListTasksResponse ListTasksByContainerInstance(string cluster, string containerInstanceId, string? nextToken);

    ListTasksResponse ListTasksByService(string cluster, string serviceName, string? nextToken);

    IEnumerable<PSObject> DescribeTasks(string cluster, IEnumerable<string> taskIds,
        IEnumerable<string>? include = null);

    void StopTask(string cluster, string taskId, string? reason);
    void DeleteService(string cluster, string serviceName, bool force);
}