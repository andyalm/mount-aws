using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Iam;

public class PolicyArn : TypedString
{
    public static PolicyArn Create(CallerIdentity callerIdentity, string pathAndName)
    {
        return new PolicyArn($"arn:aws:iam::{callerIdentity.AccountId}:policy/{pathAndName}");
    }

    public PolicyArn(string value) : base(value)
    {
    }
}