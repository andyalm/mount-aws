using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.Ecr;

public class RepositoryItem : AwsItem
{
    public RepositoryItem(string parentPath, string prefix) : base(parentPath, new PSObject())
    {
        ItemName = prefix;
        ItemType = EcrItemTypes.Directory;
    }

    public RepositoryItem(string parentPath, PSObject repository) : base(parentPath, repository)
    {
        ItemName = ItemPath.GetLeaf(repository.Property<string>("RepositoryName")!);
        ItemType = EcrItemTypes.Repository;
        RepositoryUri = repository.Property<string>(nameof(RepositoryUri));
    }
    public override string ItemName { get; }
    public override string ItemType { get; }
    public override bool IsContainer => true;
    public string? RepositoryUri { get; }
}