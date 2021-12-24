using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.Ecr;

public class RepositoryItem : Item
{
    public RepositoryItem(string parentPath, string prefix) : base(parentPath, new PSObject())
    {
        ItemName = prefix;
        ItemType = "Directory";
    }

    public RepositoryItem(string parentPath, PSObject repository) : base(parentPath, repository)
    {
        ItemName = ItemPath.GetLeaf(repository.Property<string>("RepositoryName")!);
        ItemType = "Repository";
        RepositoryUri = repository.Property<string>(nameof(RepositoryUri));
    }
    public override string ItemName { get; }
    public override string ItemType { get; }
    public override string TypeName => typeof(RepositoryItem).FullName!;
    public override bool IsContainer => true;
    public string? RepositoryUri { get; }
}