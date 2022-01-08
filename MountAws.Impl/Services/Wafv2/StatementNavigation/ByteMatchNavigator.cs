using System.Text;
using Amazon.WAFV2.Model;

namespace MountAws.Services.Wafv2.StatementNavigation;

public class ByteMatchNavigator : StatementNavigator<ByteMatchStatement>
{
    public ByteMatchNavigator(ByteMatchStatement byteMatch) : base(byteMatch)
    {
        SearchString = Encoding.UTF8.GetString(byteMatch.SearchString.ToArray());
        var fieldToMatch = byteMatch.FieldToMatch.ToNavigator();
        Description = $"{fieldToMatch.Name} {byteMatch.PositionalConstraint.Value} {SearchString}";
    }
    
    public string SearchString { get; }
    public override string Description { get; }
}