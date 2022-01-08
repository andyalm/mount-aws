using Amazon.WAFV2;
using Amazon.WAFV2.Model;

namespace MountAws.Services.Wafv2.StatementNavigation;

public class IPSetReferenceNavigator : StatementNavigator<IPSetReferenceStatement>
{
    public IPSetReferenceNavigator(IPSetReferenceStatement statement, IAmazonWAFV2 wafv2) : base(statement)
    {
        IPSet = wafv2.GetIPSet(statement.ARN);
        Description = $"{IPSet.Addresses.Count} ips: {IPSet.Description}";
    }
    
    public IPSet IPSet { get; }

    public override string Description { get; }
}