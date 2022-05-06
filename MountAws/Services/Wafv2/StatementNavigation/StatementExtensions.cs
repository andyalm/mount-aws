using System.Text;
using Amazon.WAFV2;
using Amazon.WAFV2.Model;
using MountAnything;

namespace MountAws.Services.Wafv2.StatementNavigation;

public static class StatementExtensions
{
    public static IStatementNavigator ToNavigator(this Statement statement, IAmazonWAFV2 wafv2, int position = 0)
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
                    AndStatement and => new AndNavigator(and, position, wafv2),
                    OrStatement or => new OrNavigator(or, position, wafv2),
                    NotStatement not => new NotNavigator(not, position, wafv2),
                    ManagedRuleGroupStatement managedRule => new ManagedRuleGroupNavigator(managedRule, position),
                    RuleGroupReferenceStatement reference => new RuleGroupReferenceNavigator(reference, position),
                    RegexMatchStatement regex => new RegexMatchNavigator(regex, position),
                    RegexPatternSetReferenceStatement regexRef => new RegexReferenceNavigator(regexRef, position, wafv2),
                    ByteMatchStatement byteMatch => new ByteMatchNavigator(byteMatch, position),
                    LabelMatchStatement labelMatch => new LabelMatchNavigator(labelMatch, position),
                    RateBasedStatement rateBased => new RateBasedNavigator(rateBased, position),
                    IPSetReferenceStatement ipSet => new IPSetReferenceNavigator(ipSet, position, wafv2),

                    _ => new DefaultStatementNavigator(statementValue, position)
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
                    SingleHeader singleHeader => new SingleHeaderNavigator(singleHeader, 0),
                    _ => new FieldToMatchNavigator(value, 0)
                };
            }
        }

        throw new ArgumentException("FieldToMatch bodies are all null");
    }

    public static string UniqueName(this IStatementNavigator statement)
    {
        if (statement.Position <= 0)
        {
            return statement.Name;
        }
        else
        {
            return $"{statement.Name}{statement.Position:00}";
        }
    }

    public static IStatementNavigator? StatementAt(this IStatementNavigator statement, ItemPath statementPath)
    {
        if (statementPath.IsRoot)
        {
            return statement;
        }

        var nextItem = statementPath.Parts[0];
        var matchingChild = statement.GetChildren()
            .SingleOrDefault(s => s.UniqueName().Equals(nextItem, StringComparison.OrdinalIgnoreCase));

        if (matchingChild == null)
        {
            return null;
        }

        if (statementPath.Parts.Length == 1)
        {
            return matchingChild;
        }

        var nextPath = ItemPath.Root.Combine(statementPath.Parts[1..]);

        return matchingChild.StatementAt(nextPath);
    }
    
    public static string? PascalToKebabCase(this string? source)
    {
        if (source is null) return null;

        if (source.Length == 0) return string.Empty;

        StringBuilder builder = new StringBuilder();

        for (var i = 0; i < source.Length; i++)
        {
            if (char.IsLower(source[i])) // if current char is already lowercase
            {
                builder.Append(source[i]);
            }
            else if (i == 0) // if current char is the first char
            {
                builder.Append(char.ToLower(source[i]));
            }
            else if (char.IsDigit(source[i]) && !char.IsDigit(source[i - 1])) // if current char is a number and the previous is not
            {
                builder.Append('-');
                builder.Append(source[i]);
            }
            else if (char.IsDigit(source[i])) // if current char is a number and previous is
            {
                builder.Append(source[i]);
            }
            else if (char.IsLower(source[i - 1])) // if current char is upper and previous char is lower
            {
                builder.Append('-');
                builder.Append(char.ToLower(source[i]));
            }
            else if (i + 1 == source.Length || char.IsUpper(source[i + 1])) // if current char is upper and next char doesn't exist or is upper
            {
                builder.Append(char.ToLower(source[i]));
            }
            else // if current char is upper and next char is lower
            {
                builder.Append('-');
                builder.Append(char.ToLower(source[i]));
            }
        }
        return builder.ToString();
    }
}