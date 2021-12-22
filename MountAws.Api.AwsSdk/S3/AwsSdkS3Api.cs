using System.Management.Automation;
using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using MountAws.Api.S3;
using ListObjectsRequest = MountAws.Api.S3.ListObjectsRequest;
using ListObjectsResponse = MountAws.Api.S3.ListObjectsResponse;

namespace MountAws.Api.AwsSdk.S3;

public class AwsSdkS3Api : IS3Api
{
    private readonly IAmazonS3 _s3;

    public AwsSdkS3Api(IAmazonS3 s3)
    {
        _s3 = s3;
    }

    public IEnumerable<string> ListBuckets()
    {
        return _s3.ListBucketsAsync()
            .GetAwaiter()
            .GetResult()
            .Buckets
            .Select(b => b.BucketName);
    }

    public Stream GetObjectStream(string bucketName, string key)
    {
        return _s3.GetObjectStreamAsync(bucketName, key, new Dictionary<string, object>())
            .GetAwaiter().GetResult();
    }

    public PSObject GetObject(string bucketName, string key)
    {
        try
        {
            return _s3.GetObjectAsync(new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            }).GetAwaiter().GetResult().ToPSObject();
        }
        catch (AmazonS3Exception ex) when(ex.StatusCode == HttpStatusCode.OK || ex.StatusCode == HttpStatusCode.Forbidden)
        {
            throw new ObjectNotFoundException(bucketName, key);
        }
    }

    public void PutObject(string bucketName, string key, string? content)
    {
        _s3.PutObjectAsync(new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            ContentBody = content
        }).GetAwaiter().GetResult();
    }

    public void DeleteObject(string bucketName, string key)
    {
        _s3.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = key
        }).GetAwaiter().GetResult();
    }

    public bool BucketExists(string bucketName)
    {
        try
        {
            return _s3.GetBucketPolicyAsync(bucketName).GetAwaiter().GetResult().HttpStatusCode == HttpStatusCode.Found;
        }
        catch (AmazonS3Exception ex) when(ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public string GetBucketPolicy(string bucketName)
    {
        return _s3.GetBucketPolicyAsync(bucketName).GetAwaiter().GetResult().Policy;
    }

    public ListObjectsResponse ListObjects(ListObjectsRequest request)
    {
        var sdkRequest = new ListObjectsV2Request
        {
            BucketName = request.BucketName,
            Prefix = request.Prefix,
            Delimiter = request.Delimiter
        };
        var sdkResponse = _s3.ListObjectsV2Async(sdkRequest)
            .GetAwaiter()
            .GetResult();

        return new ListObjectsResponse
        {
            CommonPrefixes = sdkResponse.CommonPrefixes.ToArray(),
            S3Objects = sdkResponse.S3Objects.ToPSObjects().ToArray()
        };
    }
}