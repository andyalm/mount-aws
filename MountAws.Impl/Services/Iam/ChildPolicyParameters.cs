using System.Management.Automation;
using Amazon.IdentityManagement;

namespace MountAws.Services.Iam;

public class ChildPolicyParameters
{
    [Parameter(Mandatory = false)]
    public PolicyScope? Scope { get; set; }
}

public enum PolicyScope
{
    All,
    Local,
    Aws
}