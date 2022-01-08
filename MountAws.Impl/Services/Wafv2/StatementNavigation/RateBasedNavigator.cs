using Amazon.WAFV2.Model;

namespace MountAws.Services.Wafv2.StatementNavigation;

public class RateBasedNavigator : StatementNavigator<RateBasedStatement>
{
    public RateBasedNavigator(RateBasedStatement statement) : base(statement)
    {
        Description = $"rate limit {statement.Limit} requests per ip over 5 minutes";
    }

    public override string Description { get; }
}