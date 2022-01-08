using Amazon.WAFV2.Model;
using MountAnything;

namespace MountAws.Services.Wafv2;

public static class WebAclExtensions
{
    private const string DefaultActionItemName = "default-action";
    private const string ActionItemName = "action";
    private const string OverrideActionItemName = "override-action";
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
    
    public static ActionItem? ActionItem(this Rule rule, ItemPath parentPath)
    {
        var ruleAction = rule.Action;
        if (ruleAction?.Allow != null)
        {
            return new ActionItem(parentPath, ActionItemName, ruleAction.Allow, "allow");
        }
        else if (ruleAction?.Block != null)
        {
            return new ActionItem(parentPath, ActionItemName, ruleAction.Block, "block");
        }
        else if (ruleAction?.Captcha != null)
        {
            return new ActionItem(parentPath, ActionItemName, ruleAction.Captcha, "captcha");
        }
        else if (ruleAction?.Count != null)
        {
            return new ActionItem(parentPath, ActionItemName, ruleAction.Count, "count");
        }

        return null;
    }
    
    public static ActionItem? OverrideActionItem(this Rule rule, ItemPath parentPath)
    {
        var ruleAction = rule.OverrideAction;
        if (ruleAction?.None != null)
        {
            return new ActionItem(parentPath, OverrideActionItemName, ruleAction.None, "none");
        }
        else if (ruleAction?.Count != null)
        {
            return new ActionItem(parentPath, OverrideActionItemName, ruleAction.Count, "count");
        }

        return null;
    }
    
    public static CustomHeadersItem? GetCustomHeadersChildItem(this ActionItem actionItem, ItemPath parentPath)
    {
        return actionItem.ActionObject switch
        {
            AllowAction allow => allow.GetCustomHeadersChildItem(parentPath),
            BlockAction block => block.GetCustomHeadersChildItem(parentPath),
            _ => null
        };
    }

    public static CustomHeadersItem? GetCustomHeadersChildItem(this AllowAction allow, ItemPath parentPath)
    {
        return allow.CustomRequestHandling?.InsertHeaders.Any() == true
            ? CustomHeadersHandler.CreateItem(parentPath,
                "custom-request-headers",
                "Lists the custom request headers that will be inserted by this action",
                allow.CustomRequestHandling.InsertHeaders)
            : null;
    }
    
    public static CustomHeadersItem? GetCustomHeadersChildItem(this BlockAction block, ItemPath parentPath)
    {
        return block.CustomResponse?.ResponseHeaders.Any() == true
            ? CustomHeadersHandler.CreateItem(parentPath,
                "custom-response-headers",
                "Lists the custom response headers that will be inserted by this action",
                block.CustomResponse.ResponseHeaders)
            : null;
    }
}