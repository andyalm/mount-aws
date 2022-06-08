using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;
using MountAnything;

namespace MountAws.Services.Iam;

public class PolicyNavigator : ItemNavigator<ManagedPolicy, PolicyItem>
{
    private readonly IAmazonIdentityManagementService _iam;
    private readonly PolicyScope? _policyScope;

    public PolicyNavigator(IAmazonIdentityManagementService iam, PolicyScope? policyScope)
    {
        _iam = iam;
        _policyScope = policyScope;
    }
    
    protected override PolicyItem CreateDirectoryItem(ItemPath parentPath, ItemPath directoryPath)
    {
        return new PolicyItem(parentPath, directoryPath.ToString());
    }

    protected override PolicyItem CreateItem(ItemPath parentPath, ManagedPolicy model)
    {
        return new PolicyItem(parentPath, model);
    }

    protected override ItemPath GetPath(ManagedPolicy model)
    {
        var directory = string.IsNullOrEmpty(model.Path) ? ItemPath.Root : new(model.Path);
        return directory.Combine(model.PolicyName);
    }

    protected override IEnumerable<ManagedPolicy> ListItems(ItemPath? pathPrefix)
    {
        return _iam.ListPolicies(pathPrefix?.ToString(), _policyScope);
    }
}