using Amazon.S3;
using MountAnything.Content;

namespace MountAws.Services.S3;

public class ObjectContentReader : StreamContentReader
{
    private readonly IAmazonS3 _s3;
    private readonly string _bucketName;
    private readonly string _objectKey;

    public ObjectContentReader(IAmazonS3 s3, string bucketName, string objectKey)
    {
        _s3 = s3;
        _bucketName = bucketName;
        _objectKey = objectKey;
    }

    protected override Stream CreateContentStream()
    {
        return _s3.GetObjectStream(_bucketName, _objectKey);
    }
}