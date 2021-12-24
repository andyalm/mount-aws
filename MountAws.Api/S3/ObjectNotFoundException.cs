namespace MountAws.Api.S3;

public class ObjectNotFoundException : ApplicationException
{
    public ObjectNotFoundException(string bucketName, string key) : base($"No object with key '{key}' could be found in s3 bucket '{bucketName}'")
    {
        
    }
}