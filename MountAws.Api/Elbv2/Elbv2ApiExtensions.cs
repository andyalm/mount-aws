using System.Management.Automation;
using MountAws.Api.Elbv2;
using DescribeTargetGroupsRequest = MountAws.Api.Elbv2.DescribeTargetGroupsRequest;

namespace MountAws.Services.ELBV2;

public static class Elbv2ApiExtensions
{
    public static PSObject GetTargetGroup(this IElbv2Api elbv2, string targetGroupNameOrArn)
    {
        var request = new DescribeTargetGroupsRequest();
        if (targetGroupNameOrArn.StartsWith("arn:"))
        {
            request.Arns.Add(targetGroupNameOrArn);
        }
        else
        {
            request.Names.Add(targetGroupNameOrArn);
        }
        return elbv2.DescribeTargetGroups(request).Single();
    }

    public static string TargetGroupName(string targetGroupArn)
    {
        return targetGroupArn.Split("/")[^2];
    }
}