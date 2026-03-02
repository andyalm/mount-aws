namespace MountAws.Services.AppAutoscaling;

public interface IResourceIdResolver
{
    string ResourceId { get; }
}