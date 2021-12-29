using System.Collections;
using System.Management.Automation.Provider;
using MountAws.Api.S3;

namespace MountAws.Services.S3;

public class ObjectContentWriter : IContentWriter
{
    private readonly IS3Api _s3;
    private readonly string _bucketName;
    private readonly string _objectKey;

    public ObjectContentWriter(IS3Api s3, string bucketName, string objectKey)
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
        _s3.PutObject(_bucketName, _objectKey, string.Join(Environment.NewLine, content.Cast<string>()));

        return content;
    }
}