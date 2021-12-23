using System.Management.Automation;
using System.Text;
using MountAnything;
using MountAws.Api;

namespace MountAws.Services.Elbv2;

public class RedirectActionItem : ActionItem
{
    public RedirectActionItem(string parentPath, PSObject action) : base(parentPath, action)
    {
        RedirectLocation = BuildRedirectLocation(action.Property<PSObject>("RedirectConfig")!);
    }

    public string RedirectLocation { get; }
    public override string ItemName => Property<string>("Type")!;
    public override string ItemType => Elbv2ItemTypes.RedirectAction;
    public override bool IsContainer => false;
    public override string Description => $"Redirects to {RedirectLocation}";

    public override void CustomizePSObject(PSObject psObject)
    {
        base.CustomizePSObject(psObject);
        psObject.Properties.Add(new PSNoteProperty(nameof(RedirectLocation), RedirectLocation));
    }

    public string BuildRedirectLocation(PSObject redirectConfig)
    {
        var builder = new StringBuilder();
        if (!string.IsNullOrEmpty(redirectConfig.Property<string>("Host")))
        {
            builder.Append($"{redirectConfig.Property<string>("Protocol")!.ToLower()}://");
            builder.Append(redirectConfig.Property<string>("Host"));
        }

        if (!string.IsNullOrEmpty(redirectConfig.Property<string>("Port")))
        {
            builder.Append(':');
            builder.Append(redirectConfig.Property<string>("Port"));
        }

        if (!string.IsNullOrEmpty(redirectConfig.Property<string>("Path")))
        {
            builder.Append(redirectConfig.Property<string>("Path"));
        }

        if (!string.IsNullOrEmpty(redirectConfig.Property<string>("Query")))
        {
            builder.Append("?");
            builder.Append(redirectConfig.Property<string>("Query"));
        }

        return builder.ToString();
    }
}