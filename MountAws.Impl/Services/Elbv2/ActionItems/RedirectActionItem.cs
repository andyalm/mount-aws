using System.Management.Automation;
using System.Text;
using Amazon.ElasticLoadBalancingV2.Model;
using Action = Amazon.ElasticLoadBalancingV2.Model.Action;

namespace MountAws.Services.Elbv2;

public class RedirectActionItem : ActionItem
{
    public RedirectActionItem(string parentPath, Action action) : base(parentPath, action)
    {
        RedirectLocation = BuildRedirectLocation(action.RedirectConfig);
    }

    public string RedirectLocation { get; }
    public override string ItemType => Elbv2ItemTypes.RedirectAction;
    public override bool IsContainer => false;
    public override string Description => $"Redirects to {RedirectLocation}";

    public override void CustomizePSObject(PSObject psObject)
    {
        base.CustomizePSObject(psObject);
        psObject.Properties.Add(new PSNoteProperty(nameof(RedirectLocation), RedirectLocation));
    }

    public string BuildRedirectLocation(RedirectActionConfig redirectConfig)
    {
        var builder = new StringBuilder();
        if (!string.IsNullOrEmpty(redirectConfig.Host))
        {
            builder.Append($"{redirectConfig.Protocol.ToLower()}://");
            builder.Append(redirectConfig.Host);
        }

        if (!string.IsNullOrEmpty(redirectConfig.Port))
        {
            builder.Append(':');
            builder.Append(redirectConfig.Port);
        }

        if (!string.IsNullOrEmpty(redirectConfig.Path))
        {
            builder.Append(redirectConfig.Path);
        }

        if (!string.IsNullOrEmpty(redirectConfig.Query))
        {
            builder.Append("?");
            builder.Append(redirectConfig.Query);
        }

        return builder.ToString();
    }
}