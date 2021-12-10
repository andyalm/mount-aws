using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Provider;
using Amazon;
using Amazon.EC2;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Autofac;
using Autofac.Core;
using Autofac.Features.ResolveAnything;
using MountAws.Routing;
using MountAws.Services.EC2;

namespace MountAws;
[CmdletProvider("MountAws", ProviderCapabilities.Filter | ProviderCapabilities.ExpandWildcards)]
public class MountAwsProvider : NavigationCmdletProvider, IPathHandlerContext
{
    private static readonly Cache _cache = new();
    private static readonly ILifetimeScope _container;
    private static readonly Router _router;

    protected override bool IsValidPath(string path) => true;

    public Cache Cache => _cache;
    
    bool IPathHandlerContext.Force => base.Force.IsPresent;
    
    protected override Collection<PSDriveInfo> InitializeDefaultDrives()
    {
        return new Collection<PSDriveInfo>
        {
            new("aws", ProviderInfo, ItemSeparator.ToString(),
                "Allows you to navigate aws services as a virtual filesystem", null)
        };
    }

    static MountAwsProvider()
    {
        _router = new Router()
            .MapRoot<ProfilesHandler>()
            .MapRegex<ProfileHandler>("(?<Profile>[a-z0-9-_]+)", profile =>
        {
            var chain = new CredentialProfileStoreChain();
            profile.RegisterServices((match, builder) =>
            {
                var profileName = match.Groups["Profile"].Value;
                if (chain.TryGetProfile(profileName, out var awsProfile))
                {
                    builder.RegisterInstance(awsProfile);
                }
                if (chain.TryGetAWSCredentials(profileName, out var awsCredentials))
                {
                    builder.RegisterInstance(awsCredentials);
                }
            });
            profile.MapRegex<RegionHandler>("(?<Region>[a-z0-9-]+)", region =>
            {
                region.RegisterServices((match, builder) =>
                {
                    var regionName = match.Groups["Region"].Value;
                    builder.Register<RegionEndpoint>(c => RegionEndpoint.GetBySystemName(regionName));
                });
                region.MapRegex<EC2Handler>("ec2", ec2 =>
                {
                    ec2.MapRegex<EC2InstancesHandler>("instances", instances =>
                    {
                        instances.MapRegex<EC2InstanceHandler>(@"(?<InstanceItemName>[a-z0-9-\.]+)");
                    });
                });
            });
        });
        
        var containerBuilder = new ContainerBuilder();
        containerBuilder.RegisterInstance(_router);
        containerBuilder.RegisterInstance<RegionEndpoint>(RegionEndpoint.USEast1);
        containerBuilder.RegisterInstance<AWSCredentials>(new AnonymousAWSCredentials());
        containerBuilder.RegisterType<AmazonEC2Client>().As<IAmazonEC2>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
        containerBuilder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

        _container = containerBuilder.Build();
    }

    protected override bool ItemExists(string path)
    {
        if (path.Contains("*"))
        {
            return false;
        }

        try
        {
            return WithPathHandler(path, handler => handler.Exists());
        }
        catch (Exception ex)
        {
            WriteDebug(ex.ToString());
            throw;
        }
    }

    protected override void GetItem(string path)
    {
        try
        {
            WithPathHandler(path, handler =>
            {
                var item = handler.GetItem();
                if (item != null)
                {
                    WriteAwsItem(item);
                }
            });
        }
        catch (Exception ex)
        {
            WriteDebug(ex.ToString());
            throw;
        }
    }

    protected override void GetChildItems(string path, bool recurse)
    {
        WriteDebug($"IncludeCount: {base.Include.Count}");
        WithPathHandler(path, handler =>
        {
            if (handler is IGetChildItemParameters handlerWithParams)
            {
                handlerWithParams.GetChildItemParameters = DynamicParameters;
            }
            var childItems = handler.GetChildItems(useCache: false);
            WriteAwsItems(childItems);
        });
    }

    protected override object? GetChildItemsDynamicParameters(string path, bool recurse)
    {
        return WithPathHandler(path, handler =>
        {
            if (handler is IGetChildItemParameters handlerWithParams)
            {
                return handlerWithParams.GetChildItemParameters;
            }

            return null;
        });
    }

    protected override void GetChildItems(string path, bool recurse, uint depth)
    {
        GetChildItems(path, recurse);
    }
    
    protected override bool HasChildItems(string path)
    {
        return WithPathHandler<bool?>(path, handler => handler.GetChildItems(useCache:true).Any()) ?? false;
    }

    protected override bool IsItemContainer(string path)
    {
        return WithPathHandler(path, handler => handler.GetItem()?.IsContainer) ?? false;
    }

    private void WriteAwsItems<T>(IEnumerable<T> awsItems) where T : AwsItem
    {
        foreach (var awsItem in awsItems)
        {
            WriteAwsItem(awsItem);
        }
    }
    
    private void WriteAwsItem(AwsItem awsItem)
    {
        WriteDebug($"WriteItemObject<{awsItem.TypeName}>(,{awsItem.FullPath},{awsItem.IsContainer})");
        var providerPath = ToProviderPath(awsItem.FullPath);
        WriteItemObject(awsItem.ToPipelineObject(), providerPath, awsItem.IsContainer);
    }

    private TReturn? WithPathHandler<TReturn>(string path, Func<IPathHandler,TReturn> action)
    {
        path = AwsPath.Normalize(path);
        RouteMatch? match = null;
        try
        { 
            match = _router.Match(path);
            using var lifetimeScope = _container.BeginLifetimeScope(match.ServiceRegistrations);
            var handler = (IPathHandler)lifetimeScope.Resolve(match.HandlerType,
                new NamedParameter("path", path),
                new TypedParameter(typeof(IPathHandlerContext), this));

            return action(handler);
        }
        catch (RoutingException ex)
        {
            WriteError(new ErrorRecord(ex, "1", ErrorCategory.ObjectNotFound, this));
            return default;
        }
        catch (DependencyResolutionException ex) when(match != null)
        {
            WriteDebug($"Error creating handler {match.HandlerType.Name} with path {path}");
            WriteDebug(ex.ToString());
            WriteError(new ErrorRecord(ex, "2", ErrorCategory.NotSpecified, this));
            throw;
        }
        catch (Exception ex)
        {
            WriteDebug(ex.ToString());
            WriteError(new ErrorRecord(ex, "2", ErrorCategory.NotSpecified, this));
            throw;
        }
    }
    
    private void WithPathHandler(string path, Action<IPathHandler> action)
    {
        WithPathHandler<object?>(path, handler =>
        {
            action(handler);

            return null;
        });
    }
    
    public string ToProviderPath(string path)
    {
        return $"{ItemSeparator}{path.Replace("/", ItemSeparator.ToString())}";
    }

    protected override void GetChildNames(string path, ReturnContainers returnContainers)
    {
        WriteDebug($"GetChildNames({path}, {returnContainers})");
        base.GetChildNames(path, returnContainers);
    }

    protected override string[] ExpandPath(string path)
    {
        var normalizedPath = AwsPath.Normalize(path);
        var handlerPath = AwsPath.GetParent(normalizedPath);
        var pattern = AwsPath.GetLeaf(normalizedPath);
        return WithPathHandler(handlerPath, handler =>
        {
            WriteDebug($"{handler.GetType().Name}.ExpandPath({pattern})");
            return handler.ExpandPath(pattern)
                .Select(ToProviderPath)
                .Select(p => AwsPath.GetParent(p) == "/" && p.StartsWith("/") ? p.Substring(1) : p)
                .ToArray();
        }) ?? Array.Empty<string>();

    }

    protected override bool ConvertPath(string path, string filter, ref string updatedPath, ref string updatedFilter)
    {
        WriteDebug($"ConvertPath({path}, {filter})");
        return base.ConvertPath(path, filter, ref updatedPath, ref updatedFilter);
    }

    public override string GetResourceString(string baseName, string resourceId)
    {
        WriteDebug($"GetResourceString({baseName}, {resourceId})");
        return base.GetResourceString(baseName, resourceId);
    }

    protected override string NormalizeRelativePath(string path, string basePath)
    {
        WriteDebug($"NormalizeRelativePath({path}, {basePath})");
        return base.NormalizeRelativePath(path, basePath);
    }
}
