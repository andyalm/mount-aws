using Amazon.WAFV2;
using Amazon.WAFV2.Model;

namespace MountAws.Services.Wafv2.StatementNavigation;

public class RegexReferenceNavigator : StatementNavigator<RegexPatternSetReferenceStatement>
{
    public RegexReferenceNavigator(RegexPatternSetReferenceStatement statement, int position, IAmazonWAFV2 wafv2) : base(statement, position)
    {
        RegexPatternSet = wafv2.GetRegexPatternSet(statement.ARN);
        Description = $"{statement.FieldToMatch.ToNavigator().Description} RegexPatternSet: {RegexPatternSet.Description}";
    }
    
    public RegexPatternSet RegexPatternSet { get; }
    public override string Description { get; }
}