using Amazon.IdentityManagement.Model;
using MountAnything;

namespace MountAws.Services.Iam;

public static class ModelExtensions
{
    public static IEnumerable<string> PolicyDirectories(this IEnumerable<ManagedPolicy> policies, string? pathPrefix)
    {
        pathPrefix ??= string.Empty;
        return (from policy in policies
            let policyPath = policy.ItemPath()
            where !policyPath.IsRoot && policyPath.Parent.ToString().Equals(pathPrefix, StringComparison.OrdinalIgnoreCase)
            select policyPath.Name).Distinct();
    }

    public static IEnumerable<ManagedPolicy> ChildPolicies(this IEnumerable<ManagedPolicy> policies, string? pathPrefix)
    {
        pathPrefix ??= string.Empty;
        return from policy in policies
            let policyPath = policy.ItemPath()
            where policyPath.FullName.Equals(pathPrefix, StringComparison.OrdinalIgnoreCase)
            select policy;
    }

    public static ItemPath ItemPath(this ManagedPolicy policy)
    {
        if (policy.Path?.EndsWith("/") == true)
        {
            return new ItemPath(policy.Path.TrimEnd('/'));
        }

        return new ItemPath(policy.Path ?? string.Empty);
    }
}