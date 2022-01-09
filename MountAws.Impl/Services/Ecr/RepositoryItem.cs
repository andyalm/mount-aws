using System.Management.Automation;
using Amazon.ECR.Model;
using MountAnything;

namespace MountAws.Services.Ecr;

public class RepositoryItem : AwsItem
{
    public Repository? Repository { get; }
    
    public RepositoryItem(ItemPath parentPath, string prefix) : base(parentPath, new PSObject())
    {
        ItemName = prefix;
        ItemType = EcrItemTypes.Directory;
    }

    public RepositoryItem(ItemPath parentPath, Repository repository) : base(parentPath, repository.ToPSObject())
    {
        ItemName = repository.RepositoryName.Split(ItemPath.Separator).Last();
        ItemType = EcrItemTypes.Repository;
        RepositoryUri = repository.RepositoryUri;
        Repository = repository;
        RepositoryId = repository.RegistryId;
    }
    public override string ItemName { get; }
    public override string ItemType { get; }
    public override bool IsContainer => true;
    public string? RepositoryUri { get; }
    
    public string? RepositoryId { get; }

    public override string? WebUrl => Repository != null
        ? UrlBuilder.CombineWith($"ecr/repositories/private/{RepositoryId!}/{RepositoryUri!}").ToString()
        : null;
}