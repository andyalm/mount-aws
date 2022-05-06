using System.Management.Automation;
using Microsoft.PowerShell.Commands;

namespace MountAws;

public static class PowershellJsonExtensions
{
    public static PSObject FromJsonToPSObject(this string rawJson)
    {
        var cmd = new ConvertFromJsonCommand
        {
            InputObject = rawJson
        };
        return cmd.Invoke<PSObject>().Single();
    }
}