using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Reflection;
using MountAnything;
using MountAnything.Routing;
using MountAws.Host.Abstractions;

namespace MountAws.Host;
[CmdletProvider("MountAws", ProviderCapabilities.ExpandWildcards | ProviderCapabilities.Filter)]
public class MountAwsProvider : MountAnythingProvider
{
    protected override Collection<PSDriveInfo> InitializeDefaultDrives()
    {
        return new Collection<PSDriveInfo>
        {
            new("aws", ProviderInfo, ItemSeparator.ToString(),
                "Allows you to navigate aws services as a virtual filesystem", null)
        };
    }

    public override Router CreateRouter()
    {
        var assembly = LoadImplAssembly();
        var routerFactoryType = assembly
            .GetExportedTypes()
            .Single(t => typeof(IRouterFactory).IsAssignableFrom(t));
        var routerFactory = (IRouterFactory)Activator.CreateInstance(routerFactoryType)!;

        return routerFactory.CreateRouter();
    }

    private Assembly LoadImplAssembly()
    {
        var modulePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var apiAssemblyDir = Path.Combine(modulePath, "Impl");
        var assemblyLoadContext = new ImplAssemblyLoadContext(apiAssemblyDir);
        
        return assemblyLoadContext.LoadFromAssemblyName(new AssemblyName("MountAws.Impl"));
    }
}
