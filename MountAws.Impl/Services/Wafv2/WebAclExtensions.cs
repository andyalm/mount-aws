using Amazon.WAFV2.Model;
using MountAnything;

namespace MountAws.Services.Wafv2;

public static class WebAclExtensions
{
    private const string DefaultActionItemName = "default-action";
    public static ActionItem? DefaultActionItem(this WebACL acl, ItemPath parentPath)
    {
        if (acl.DefaultAction.Allow != null)
        {
            return new ActionItem(parentPath, DefaultActionItemName, acl.DefaultAction.Allow, "allow");
        }
        else if (acl.DefaultAction.Block != null)
        {
            return new ActionItem(parentPath, DefaultActionItemName, acl.DefaultAction.Block, "block");
        }

        return null;
    }
}