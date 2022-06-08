using System.Management.Automation;
using System.Net;
using Amazon.IdentityManagement.Model;
using MountAnything;

namespace MountAws.Services.Iam;

public static class ModelExtensions
{
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
}