using Amazon.IdentityManagement.Model;

namespace MountAws.Services.Iam;

public record RolePolicyAttachment(string RoleName, string PolicyName, string PolicyArn, PolicyVersion PolicyVersion);