using Amazon.WAFV2;
using Amazon.WAFV2.Model;

namespace MountAws.Services.Wafv2.StatementNavigation;

public static class StatementExtensions
{
    public static IStatementNavigator ToNavigator(this Statement statement, IAmazonWAFV2 wafv2)
    {
        var statementProperties = statement.GetType().GetProperties()
            .Where(s => s.CanRead && s.Name.EndsWith("Statement"));
        foreach (var statementProperty in statementProperties)
        {
            var statementValue = statementProperty.GetValue(statement);
            if (statementValue != null)
            {
                return statementValue switch
                {
                    AndStatement and => new AndNavigator(and, wafv2),
                    OrStatement or => new OrNavigator(or, wafv2),
                    NotStatement not => new NotNavigator(not, wafv2),
                    ManagedRuleGroupStatement managedRule => new ManagedRuleGroupNavigator(managedRule),
                    RuleGroupReferenceStatement reference => new RuleGroupReferenceNavigator(reference),
                    RegexMatchStatement regex => new RegexMatchNavigator(regex),
                    RegexPatternSetReferenceStatement regexRef => new RegexReferenceNavigator(regexRef, wafv2),
                    ByteMatchStatement byteMatch => new ByteMatchNavigator(byteMatch),
                    LabelMatchStatement labelMatch => new LabelMatchNavigator(labelMatch),
                    RateBasedStatement rateBased => new RateBasedNavigator(rateBased),
                    IPSetReferenceStatement ipSet => new IPSetReferenceNavigator(ipSet, wafv2),

                    _ => new DefaultStatementNavigator(statementValue)
                };
            }
        }

        throw new ArgumentException($"Statement bodies are all null");
    }

    public static IStatementNavigator ToNavigator(this FieldToMatch fieldToMatch)
    {
        var properties = fieldToMatch.GetType().GetProperties()
            .Where(p => p.CanRead);

        foreach (var property in properties)
        {
            var value = property.GetValue(fieldToMatch);
            if (value != null)
            {
                return value switch
                {
                    SingleHeader singleHeader => new SingleHeaderNavigator(singleHeader),
                    _ => new FieldToMatchNavigator(value)
                };
            }
        }

        throw new ArgumentException("FieldToMatch bodies are all null");
    }
}