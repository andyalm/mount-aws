using System.Collections;
using System.Management.Automation;

namespace MountAws.Services.EC2;

public interface IEC2QueryParameters
{
    public string? IPAddress { get; }
}