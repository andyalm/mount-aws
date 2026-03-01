using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using static MountAws.PagingHelper;

namespace MountAws.Services.SecretsManager;

public static class SecretsManagerApiExtensions
{
    public static IEnumerable<SecretListEntry> ListSecrets(this IAmazonSecretsManager secretsManager)
    {
        return Paginate(nextToken =>
        {
            var response = secretsManager.ListSecretsAsync(new ListSecretsRequest
            {
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return (response.SecretList, response.NextToken);
        });
    }
    
    public static IEnumerable<SecretListEntry> ListSecrets(this IAmazonSecretsManager secretsManager, string pathPrefix)
    {
        return Paginate(nextToken =>
        {
            var response = secretsManager.ListSecretsAsync(new ListSecretsRequest
            {
                Filters = [
                    new Filter
                    {
                        Key = FilterNameStringType.Name,
                        Values = [
                            pathPrefix
                        ]
                    }
                ],
                NextToken = nextToken
            }).GetAwaiter().GetResult();

            return (response.SecretList, response.NextToken);
        });
    }

    public static DescribeSecretResponse DescribeSecret(this IAmazonSecretsManager secretsManager, string secretId)
    {
        return secretsManager.DescribeSecretAsync(new DescribeSecretRequest
        {
            SecretId = secretId
        }).GetAwaiter().GetResult();
    }

    public static GetSecretValueResponse GetSecretValue(this IAmazonSecretsManager secretsManager, string secretId)
    {
        return secretsManager.GetSecretValueAsync(new GetSecretValueRequest
        {
            SecretId = secretId
        }).GetAwaiter().GetResult();
    }

    public static DescribeSecretResponse? DescribeSecretOrDefault(this IAmazonSecretsManager secretsManager, string secretId)
    {
        try
        {
            return secretsManager.DescribeSecret(secretId);
        }
        catch (ResourceNotFoundException)
        {
            return null;
        }
        catch (AmazonSecretsManagerException)
        {
            return null;
        }
    }

    public static void PutSecretValue(this IAmazonSecretsManager secretsManager, string secretId, string secretString)
    {
        secretsManager.PutSecretValueAsync(new PutSecretValueRequest
        {
            SecretId = secretId,
            SecretString = secretString
        }).GetAwaiter().GetResult();
    }
}
