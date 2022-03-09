using Amazon.IdentityManagement;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Iam;

public class PoliciesHandler : PathHandler, IGetChildItemParameters<ChildPolicyParameters>
{
    private readonly IAmazonIdentityManagementService _iam;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "policies",
            "Navigate IAM policies");
    }
    
    public PoliciesHandler(ItemPath path, IPathHandlerContext context, IAmazonIdentityManagementService iam) : base(path, context)
    {
        _iam = iam;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _iam.ListChildPolicyItems(Path, scope: GetChildItemParameters.Scope);
    }

    public ChildPolicyParameters GetChildItemParameters { get; set; } = new();
}