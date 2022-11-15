using Amazon.ElastiCache.Model;

namespace MountAws.Services.Elasticache;

public static class ModelExtensions
{
    public static string? ToAddressAndPortString(this Endpoint? endpoint)
    {
        if (endpoint == null)
        {
            return null;
        }
        
        return $"{endpoint.Address}:{endpoint.Port}";
    }
}