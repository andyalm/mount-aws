using Amazon.WAFV2.Model;

namespace MountAws.Services.Wafv2;

public static class Wafv2ModelExtensions
{
    public static bool IsGlobal(this WebACLSummary webAcl)
    {
        return webAcl.ARN.Split(":").Last().Split("/").First() == "global";
    }

    public static string RegionName(this WebACLSummary webAcl)
    {
        return webAcl.ARN.Split(":")[3];
    }
}