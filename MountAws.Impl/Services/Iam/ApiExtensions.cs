using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;
using MountAnything;
using MountAws.Services.Core;
using static MountAws.PagingHelper;

namespace MountAws.Services.Iam;

public static class ApiExtensions
{
    public static IEnumerable<ManagedPolicy> ListPolicies(this IAmazonIdentityManagementService iam, string? pathPrefix = null, PolicyScope? scope = null)
    {
        return Paginate(nextToken =>
        {
            var response = iam.ListPoliciesAsync(new ListPoliciesRequest
            {
                Marker = nextToken,
                PathPrefix = pathPrefix.ToApiCompliantPrefix(),
                Scope = scope.ToApiPolicyScope()
            }).GetAwaiter().GetResult();

            return (response.Policies, response.Marker);
        });
    }

    public static IEnumerable<PolicyItem> ListChildPolicyItems(this IAmazonIdentityManagementService iam,
        ItemPath parentPath, string? pathPrefix = null, PolicyScope? scope = null)
    {
        var policies = iam.ListPolicies(pathPrefix, scope).ToArray();
        var directories = policies
            .Directories(pathPrefix);
        var childPolicies = policies.ChildPolicies(pathPrefix);

        return directories.OrderBy(d => d)
            .Select(directory => new PolicyItem(parentPath, directory))
            .Concat(childPolicies.OrderBy(p => p.PolicyName).Select(p => new PolicyItem(parentPath, p)));
    }
    
    public static IEnumerable<RoleItem> ListChildRoleItems(this IAmazonIdentityManagementService iam,
        ItemPath parentPath, string? pathPrefix = null)
    {
        var roles = iam.ListRoles(pathPrefix).ToArray();
        var directories = roles.Directories(pathPrefix);
        var childRoles = roles.ChildRoles(pathPrefix);

        return directories.OrderBy(d => d)
            .Select(directory => new RoleItem(parentPath, directory))
            .Concat(childRoles.OrderBy(r => r.RoleName).Select(p => new RoleItem(parentPath, p)));
    }

    public static ManagedPolicy GetPolicy(this IAmazonIdentityManagementService iam, CallerIdentity callerIdentity, string pathAndName)
    {
        return iam.GetPolicyAsync(new GetPolicyRequest
        {
            PolicyArn = PolicyArn.Create(callerIdentity, pathAndName)
        }).GetAwaiter().GetResult().Policy;
    }
    
    public static ManagedPolicy? GetPolicyOrDefault(this IAmazonIdentityManagementService iam, CallerIdentity callerIdentity, string pathAndName)
    {
        try
        {
            return iam.GetPolicy(callerIdentity, pathAndName);
        }
        catch (NoSuchEntityException)
        {
            return null;
        }
    }

    public static Role? GetRoleOrDefault(this IAmazonIdentityManagementService iam, string roleName)
    {
        try
        {
            return iam.GetRoleAsync(new GetRoleRequest
            {
                RoleName = roleName
            }).GetAwaiter().GetResult().Role;
        }
        catch (NoSuchEntityException)
        {
            return null;
        }
    }

    public static IEnumerable<Role> ListRoles(this IAmazonIdentityManagementService iam, string? pathPrefix = null)
    {
        return Paginate(nextToken =>
        {
            var response = iam.ListRolesAsync(new ListRolesRequest
            {
                Marker = nextToken,
                PathPrefix = pathPrefix.ToApiCompliantPrefix()
            }).GetAwaiter().GetResult();

            return (response.Roles, response.Marker);
        });
    }

    private static string? ToApiCompliantPrefix(this string? pathPrefix)
    {
        if (pathPrefix == null)
        {
            return pathPrefix;
        }

        if (!pathPrefix.StartsWith("/"))
        {
            pathPrefix = $"/{pathPrefix}";
        }

        if (!pathPrefix.EndsWith("/"))
        {
            pathPrefix = $"{pathPrefix}/";
        }

        return pathPrefix;
    }

    private static PolicyScopeType? ToApiPolicyScope(this PolicyScope? policyScope)
    {
        return policyScope switch
        {
            PolicyScope.All => PolicyScopeType.All,
            PolicyScope.Aws => PolicyScopeType.AWS,
            PolicyScope.Local => PolicyScopeType.Local,
            _ => null
        };
    }
}