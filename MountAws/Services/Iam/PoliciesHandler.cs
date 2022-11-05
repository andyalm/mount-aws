using Amazon.IdentityManagement;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Iam;

public class PoliciesHandler : PathHandler, IGetChildItemParameters<ChildPolicyParameters>
{
    private readonly Lazy<PolicyNavigator> _navigator;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "policies",
            "Navigate IAM policies");
    }
    
    public PoliciesHandler(ItemPath path, IPathHandlerContext context, IAmazonIdentityManagementService iam) : base(path, context)
    {
        _navigator = new Lazy<PolicyNavigator>(() => new PolicyNavigator(iam, GetChildItemParameters.Scope));
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _navigator.Value.ListChildItems(Path);
    }

    public ChildPolicyParameters GetChildItemParameters { get; set; } = new();
}