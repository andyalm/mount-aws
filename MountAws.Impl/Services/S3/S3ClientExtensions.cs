using MountAnything;
using MountAws.Api.S3;
using ListObjectsRequest = MountAws.Api.S3.ListObjectsRequest;

namespace MountAws.Services.S3;

public static class S3ClientExtensions
{
    public static IEnumerable<Item> ListChildItems(this IS3Api s3,
        string bucketName, string parentPath,
        string? prefix = null,
        int? maxResults = null)
    {
        var request = new ListObjectsRequest
        {
            Delimiter = "/",
            BucketName = bucketName,
            Prefix = prefix
        };
        if (maxResults != null)
        {
            request.MaxKeys = maxResults.Value;
        }

        var response = s3.ListObjects(request);

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