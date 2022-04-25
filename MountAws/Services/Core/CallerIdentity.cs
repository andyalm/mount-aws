namespace MountAws.Services.Core;

public record CallerIdentity(string AccountId, string UserId, string Arn) {}