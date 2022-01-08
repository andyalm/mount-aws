using Amazon.WAFV2.Model;

namespace MountAws.Services.Wafv2.StatementNavigation;

public class RegexMatchNavigator : StatementNavigator<RegexMatchStatement>
{
    public RegexMatchNavigator(RegexMatchStatement regex) : base(regex)
    {
        var fieldToMatch = regex.FieldToMatch.ToNavigator();
        Description = $"{fieldToMatch.Name} ~= {regex.RegexString}";
    }

    public override string Description { get; }
}