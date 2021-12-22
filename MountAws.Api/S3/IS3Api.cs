using System.Management.Automation;

namespace MountAws.Api.S3;

public interface IS3Api
{
    IEnumerable<string> ListBuckets();

    Stream GetObjectStream(string bucketName, string key);

    PSObject GetObject(string bucketName, string key);

    void PutObject(string bucketName, string key, string? content);

    void DeleteObject(string bucketName, string key);

    bool BucketExists(string bucketName);

    string GetBucketPolicy(string bucketName);

    ListObjectsResponse ListObjects(ListObjectsRequest request);
}