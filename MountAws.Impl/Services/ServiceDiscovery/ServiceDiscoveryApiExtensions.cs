using Amazon.ServiceDiscovery;
using Amazon.ServiceDiscovery.Model;
using static MountAws.PagingHelper;

namespace MountAws.Services.ServiceDiscovery;

public static class ServiceDiscoveryApiExtensions
{
    public static IEnumerable<NamespaceSummary> ListNamespaces(this IAmazonServiceDiscovery serviceDiscovery)
    {
        return Paginate(nextToken =>
        {
            var response = serviceDiscovery.ListNamespacesAsync(new ListNamespacesRequest
            {
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return (response.Namespaces, response.NextToken);
        });
    }

    public static Namespace GetNamespace(this IAmazonServiceDiscovery serviceDiscovery, string id)
    {
        return serviceDiscovery.GetNamespaceAsync(new GetNamespaceRequest
        {
            Id = id
        }).GetAwaiter().GetResult().Namespace;
    }

    public static IEnumerable<ServiceSummary> ListServices(this IAmazonServiceDiscovery serviceDiscovery, string namespaceId)
    {
        return Paginate(nextToken =>
        {
            var response = serviceDiscovery.ListServicesAsync(new ListServicesRequest
            {
                Filters = new List<ServiceFilter>
                {
                    new()
                    {
                        Name = ServiceFilterName.NAMESPACE_ID,
                        Values = new List<string> { namespaceId }
                    }
                }
            }).GetAwaiter().GetResult();

            return (response.Services, response.NextToken);
        });
    }
    
    public static Service GetService(this IAmazonServiceDiscovery serviceDiscovery, string id)
    {
        return serviceDiscovery.GetServiceAsync(new GetServiceRequest
        {
            Id = id
        }).GetAwaiter().GetResult().Service;
    }

    public static IEnumerable<InstanceSummary> ListInstances(this IAmazonServiceDiscovery serviceDiscovery, string serviceId)
    {
        return Paginate(nextToken =>
        {
            var response = serviceDiscovery.ListInstancesAsync(new ListInstancesRequest
            {
                ServiceId = serviceId,
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return (response.Instances, response.NextToken);
        });
    }

    public static Instance GetInstance(this IAmazonServiceDiscovery serviceDiscovery, string serviceId, string instanceId)
    {
        return serviceDiscovery.GetInstanceAsync(new GetInstanceRequest
        {
            ServiceId = serviceId,
            InstanceId = instanceId
        }).GetAwaiter().GetResult().Instance;
    }
}