namespace MountAws.Api.Elbv2;

public class TargetGroupNotFoundException : ApplicationException
{
    public TargetGroupNotFoundException(string message) : base(message)
    {
        
    }
}