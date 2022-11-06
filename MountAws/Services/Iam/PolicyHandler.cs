using System.Management.Automation.Provider;
using System.Net;
using System.Text;
using Amazon.IdentityManagement;
using MountAnything;
using MountAnything.Content;
using MountAws.Services.Core;

namespace MountAws.Services.Iam;

public class PolicyHandler : PathHandler, IGetChildItemParameters<ChildPolicyParameters>, IContentReaderHandler
{
    private readonly IAmazonIdentityManagementService _iam;
    private readonly Lazy<PolicyNavigator> _navigator;
    private readonly IamItemPath _policyPath;
    private readonly CallerIdentity _callerIdentity;

    public PolicyHandler(ItemPath path, IPathHandlerContext context, IAmazonIdentityManagementService iam, IamItemPath policyPath, CallerIdentity callerIdentity) : base(path, context)
    {
        _iam = iam;
        _policyPath = policyPath;
        _callerIdentity = callerIdentity;
        _navigator = new Lazy<PolicyNavigator>(() => new PolicyNavigator(iam, GetChildItemParameters.Scope));
    }

    protected override IItem? GetItemImpl()
    {
        var policy = _iam.GetPolicyOrDefault(_callerIdentity, _policyPath.ToString());
        if (policy != null)
        {
            return new PolicyItem(ParentPath, policy);
        }

        return new PolicyItem(ParentPath, ItemName);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return GetItem() switch
        {
            PolicyItem { ItemType: IamItemTypes.Directory } directory => GetChildPoliciesWithinDirectory(),
            PolicyItem { ItemType: IamItemTypes.Policy } policy => GetPolicyChildren(),
            _ => Enumerable.Empty<IItem>()
        };
    }

    private IEnumerable<IItem> GetPolicyChildren()
    {
        yield break;
    }

    private IEnumerable<IItem> GetChildPoliciesWithinDirectory()
    {
        return _navigator.Value.ListChildItems(Path, _policyPath.Path);
    }

    protected override bool CacheChildren => GetChildItemParameters.Scope == null;

    public ChildPolicyParameters GetChildItemParameters { get; set; } = new();
    public Stream GetContent()
    {
        if (GetItem() is PolicyItem { Arn: not null, DefaultVersionId: not null } policyItem)
        {
            var policyDocument = _iam.GetPolicyVersion(policyItem.Arn, policyItem.DefaultVersionId).Document;

            return new MemoryStream(Encoding.UTF8.GetBytes(WebUtility.UrlDecode(policyDocument)));
        }

        throw new InvalidOperationException("This item does not support reading content");
    }
}