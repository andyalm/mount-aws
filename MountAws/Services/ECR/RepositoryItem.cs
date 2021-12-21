using Amazon.ECR.Model;
using MountAnything;

namespace MountAws.Services.ECR;

public class RepositoryItem : Item
{
    public RepositoryItem(string parentPath, string prefix) : base(parentPath)
    {
        ItemName = prefix;
        UnderlyingObject = new { };
        ItemType = "Directory";
    }

    public RepositoryItem(string parentPath, Repository repository) : base(parentPath)
    {
        ItemName = ItemPath.GetLeaf(repository.RepositoryName);
        UnderlyingObject = repository;
        ItemType = "Repository";
        Repository = repository;
        RepositoryUri = repository.RepositoryUri;
    }

    public Repository? Repository { get; }
    public override string ItemName { get; }
    public override object UnderlyingObject { get; }
    public override string ItemType { get; }
    public override string TypeName => typeof(RepositoryItem).FullName!;
    public override bool IsContainer => true;
    public string? RepositoryUri { get; }
}