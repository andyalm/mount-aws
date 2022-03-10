using Amazon.IdentityManagement.Model;

namespace MountAws.Services.Iam;

public record EntityPolicyAttachment(string EntityName, string PolicyName, string PolicyArn, PolicyVersion PolicyVersion);

public record EmbeddedPolicy(string EntityName, string PolicyName, string PolicyDocument);