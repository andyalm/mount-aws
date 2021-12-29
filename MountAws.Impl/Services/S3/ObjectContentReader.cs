using MountAnything.Content;
using MountAws.Api.S3;

namespace MountAws.Services.S3;

public class ObjectContentReader : StreamContentReader
{
    private readonly IS3Api _s3;
    private readonly string _bucketName;
    private readonly string _objectKey;

    public ObjectContentReader(IS3Api s3, string bucketName, string objectKey)
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