using System.Collections;
using System.Management.Automation.Provider;
using Amazon.S3;
using Amazon.S3.Model;

namespace MountAws.Services.S3;

public class ObjectContentWriter : IContentWriter
{
    private readonly IAmazonS3 _s3;
    private readonly string _bucketName;
    private readonly string _objectKey;

    public ObjectContentWriter(IAmazonS3 s3, string bucketName, string objectKey)
    {
        _s3 = s3;
        _bucketName = bucketName;
        _objectKey = objectKey;
    }

    public void Dispose()
    {
        
    }

    public void Close()
    {
        
    }

    public void Seek(long offset, SeekOrigin origin)
    {
        
    }

    public IList Write(IList content)
    {
        _s3.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = _objectKey,
            ContentBody = string.Join(Environment.NewLine, content.Cast<string>()),
        }).GetAwaiter().GetResult();

        return content;
    }
}