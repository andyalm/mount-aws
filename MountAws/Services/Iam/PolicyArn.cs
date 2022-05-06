using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Iam;

public record PolicyArn(string Value) : TypedString(Value)
{
    public static PolicyArn Create(CallerIdentity callerIdentity, string pathAndName)
    {
        return new PolicyArn($"arn:aws:iam::{callerIdentity.AccountId}:policy/{pathAndName}");
    }

    public override string ToString()
    {
        return Value;
    }
}