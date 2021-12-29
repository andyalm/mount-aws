using System.Management.Automation;
using Amazon.ECR.Model;
using MountAnything;

namespace MountAws.Services.Ecr;

public class RepositoryItem : AwsItem
{
    public Repository? Repository { get; }
    
    public RepositoryItem(string parentPath, string prefix) : base(parentPath, new PSObject())
    {
        ItemName = prefix;
        ItemType = EcrItemTypes.Directory;
    }

    public RepositoryItem(string parentPath, Repository repository) : base(parentPath, repository.ToPSObject())
    {
        ItemName = ItemPath.GetLeaf(repository.RepositoryName);
        ItemType = EcrItemTypes.Repository;
        RepositoryUri = repository.RepositoryUri;
        Repository = repository;
    }
    public override string ItemName { get; }
    public override string ItemType { get; }
    public override bool IsContainer => true;
    public string? RepositoryUri { get; }
}