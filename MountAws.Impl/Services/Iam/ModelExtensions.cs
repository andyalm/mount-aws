using System.Management.Automation;
using System.Net;
using Amazon.IdentityManagement.Model;
using MountAnything;

namespace MountAws.Services.Iam;

public static class ModelExtensions
{
    public static IEnumerable<string> Directories(this IEnumerable<ManagedPolicy> policies, string? pathPrefix)
    {
        return policies.Directories(pathPrefix, p => p.Path);
    }
    
    public static IEnumerable<string> Directories(this IEnumerable<Role> roles, string? pathPrefix)
    {
        return roles.Directories(pathPrefix, r => r.Path);
    }
    
    public static IEnumerable<string> Directories(this IEnumerable<User> users, string? pathPrefix)
    {
        return users.Directories(pathPrefix, r => r.Path);
    }
    
    private static IEnumerable<string> Directories<T>(this IEnumerable<T> objects, string? pathPrefix, Func<T,string> getPath)
    {
        pathPrefix ??= string.Empty;
        return (from obj in objects
            let rolePath = getPath(obj).ToItemPath()
            where !rolePath.IsRoot && rolePath.Parent.ToString().Equals(pathPrefix, StringComparison.OrdinalIgnoreCase)
            select rolePath.Name).Distinct();
    }

    public static IEnumerable<ManagedPolicy> ChildPolicies(this IEnumerable<ManagedPolicy> policies, string? pathPrefix)
    {
        return policies.Children(pathPrefix, p => p.Path);
    }
    
    public static IEnumerable<Role> ChildRoles(this IEnumerable<Role> roles, string? pathPrefix)
    {
        return roles.Children(pathPrefix, r => r.Path);
    }
    
    public static IEnumerable<User> ChildUsers(this IEnumerable<User> users, string? pathPrefix)
    {
        return users.Children(pathPrefix, r => r.Path);
    }
    
    public static IEnumerable<TModel> Children<TModel>(this IEnumerable<TModel> models, string? pathPrefix, Func<TModel, string> getPath)
    {
        pathPrefix ??= string.Empty;
        return from model in models
            let modelPath = getPath(model).ToItemPath()
            where modelPath.FullName.Equals(pathPrefix, StringComparison.OrdinalIgnoreCase)
            select model;
    }
    

    public static ItemPath ItemPath(this ManagedPolicy policy)
    {
        return policy.Path.ToItemPath();
    }

    private static ItemPath ItemPath(this Role role)
    {
        return role.Path.ToItemPath();
    }

    public static IEnumerable<PSObject> Statements(this GetUserPolicyResponse userPolicy)
    {
        return WebUtility.UrlDecode(userPolicy.PolicyDocument)
            .FromJsonToPSObject()
            .Property<IEnumerable<PSObject>>("Statement")!;
    }
    
    public static IEnumerable<PSObject> Statements(this GetRolePolicyResponse rolePolicy)
    {
        return WebUtility.UrlDecode(rolePolicy.PolicyDocument)
            .FromJsonToPSObject()
            .Property<IEnumerable<PSObject>>("Statement")!;
    }
    
    public static IEnumerable<PSObject> Statements(this EntityPolicyAttachment rolePolicyAttachment)
    {
        return WebUtility.UrlDecode(rolePolicyAttachment.PolicyVersion.Document)
            .FromJsonToPSObject()
            .Property<IEnumerable<PSObject>>("Statement")!;
    }

    private static ItemPath ToItemPath(this string path)
    {
        if (path?.EndsWith("/") == true)
        {
            return new ItemPath(path.TrimEnd('/'));
        }

        return new ItemPath(path ?? string.Empty);
    }
}