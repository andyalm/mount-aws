namespace MountAws.Services.Ec2;

public class ImageNotFoundException : ApplicationException
{
    public ImageNotFoundException(string message) : base(message)
    {
        
    }
}