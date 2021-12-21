using Amazon.S3;
using Amazon.S3.Model;
using MountAnything;

namespace MountAws.Services.S3;

public static class S3ClientExtensions
{
    public static IEnumerable<Item> ListChildItems(this IAmazonS3 s3,
        string bucketName, string parentPath,
        string? prefix = null,
        int? maxResults = null)
    {
        var request = new ListObjectsV2Request
        {
            Delimiter = "/",
            BucketName = bucketName,
            Prefix = prefix
        };
        if (maxResults != null)
        {
            request.MaxKeys = maxResults.Value;
        }
        var response = s3.ListObjectsV2Async(request)
            .GetAwaiter()
            .GetResult();

        foreach (var commonPrefix in response.CommonPrefixes)
        {
            yield return new ObjectItem(parentPath, commonPrefix);
        }

        foreach (var s3Object in response.S3Objects)
        {
            yield return new ObjectItem(parentPath, s3Object);
        }
    }
}