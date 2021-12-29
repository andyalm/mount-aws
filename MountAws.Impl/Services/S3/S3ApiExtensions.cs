using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using MountAnything;

namespace MountAws.Services.S3;

public static class S3ApiExtensions
{
    public static IEnumerable<string> ListBuckets(this IAmazonS3 s3)
    {
        return s3.ListBucketsAsync()
            .GetAwaiter()
            .GetResult()
            .Buckets
            .Select(b => b.BucketName);
    }

    public static Stream GetObjectStream(this IAmazonS3 s3, string bucketName, string key)
    {
        return s3.GetObjectStreamAsync(bucketName, key, new Dictionary<string, object>())
            .GetAwaiter().GetResult();
    }

    public static GetObjectResponse GetObject(this IAmazonS3 s3, string bucketName, string key)
    {
        try
        {
            return s3.GetObjectAsync(new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            }).GetAwaiter().GetResult();
        }
        catch (AmazonS3Exception ex) when(ex.StatusCode == HttpStatusCode.OK || ex.StatusCode == HttpStatusCode.Forbidden)
        {
            throw new ObjectNotFoundException(bucketName, key);
        }
    }

    public static void PutObject(this IAmazonS3 s3, string bucketName, string key, string? content)
    {
        s3.PutObjectAsync(new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            ContentBody = content
        }).GetAwaiter().GetResult();
    }

    public static void DeleteObject(this IAmazonS3 s3, string bucketName, string key)
    {
        s3.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = key
        }).GetAwaiter().GetResult();
    }

    public static bool BucketExists(this IAmazonS3 s3, string bucketName)
    {
        try
        {
            return s3.GetBucketPolicyAsync(bucketName).GetAwaiter().GetResult().HttpStatusCode == HttpStatusCode.Found;
        }
        catch (AmazonS3Exception ex) when(ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public static string GetBucketPolicy(this IAmazonS3 s3, string bucketName)
    {
        return s3.GetBucketPolicyAsync(bucketName).GetAwaiter().GetResult().Policy;
    }

    public static ListObjectsV2Response ListObjects(this IAmazonS3 s3, ListObjectsRequest request)
    {
        var sdkRequest = new ListObjectsV2Request
        {
            BucketName = request.BucketName,
            Prefix = request.Prefix,
            Delimiter = request.Delimiter
        };
        return s3.ListObjectsV2Async(sdkRequest)
            .GetAwaiter()
            .GetResult();
    }
    
    public static IEnumerable<IItem> ListChildItems(this IAmazonS3 s3,
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