namespace MountAws.Api.Ecr;

public class RepositoryNotFoundException : ApplicationException
{
    public RepositoryNotFoundException(string repositoryName) : base($"A repository with path {repositoryName} could not be found")
    {
        
    }
}