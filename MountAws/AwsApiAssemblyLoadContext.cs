using System.Reflection;
using System.Runtime.Loader;

namespace MountAws;

internal class AwsApiAssemblyLoadContext : AssemblyLoadContext
{
    public string DependencyPath { get; }

    public AwsApiAssemblyLoadContext(string dependencyDirPath)
    {
        DependencyPath = dependencyDirPath;
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (!assemblyName.Name!.Equals("MountAws.Api.AwsSdk", StringComparison.OrdinalIgnoreCase) &&
            !assemblyName.Name!.StartsWith("AWSSDK.", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }
        
        string assemblyPath = Path.Combine(DependencyPath, $"{assemblyName.Name}.dll");
        return LoadFromAssemblyPath(assemblyPath);
    }
}