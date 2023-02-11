using System.Management.Automation;

namespace MountAws.Services.Rds;

public class InstancesParameters
{
    [Parameter]
    public string? Engine { get; set; }
}