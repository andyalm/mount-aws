using MountAws.Api;

namespace MountAws.Services.ECR;

public class RepositoryPath : TypedString
{
    public RepositoryPath(string value) : base(value) { }
}