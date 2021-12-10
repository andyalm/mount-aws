using System.Collections;
using System.Management.Automation;

namespace MountAws.Services.EC2;

public class EC2QueryParameters : IEC2QueryParameters
{
    [Parameter]
    [Alias("ip")]
    public string? IPAddress { get; set; }
}