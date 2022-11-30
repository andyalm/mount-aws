namespace MountAws;

public record Endpoint(string IpAddress, int? Port = null)
{
    public override string ToString()
    {
        if (Port != null)
        {
            return $"{IpAddress}:{Port}";
        }
        else
        {
            return IpAddress;
        }
    }
}