﻿using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Provider;
using Amazon.Runtime.CredentialManagement;
using Autofac;
using Autofac.Features.ResolveAnything;
using MountAws.Routing;
using MountAws.Services.EC2;

namespace MountAws;
[CmdletProvider("MountAws", ProviderCapabilities.None)]
public class MountAwsProvider : NavigationCmdletProvider, IPathHandlerContext
{
    private static readonly Cache _cache = new();
    private static readonly ILifetimeScope _container;

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
        var containerBuilder = new ContainerBuilder();
        containerBuilder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

        _container = containerBuilder.Build();

        Router.Instance.MapRoot<ProfilesHandler>()
            .MapRegex<ProfileHandler>("(?<Profile>[a-z0-9-_]+)", route =>
        {
            route.RegisterServices((match, builder) =>
            {
                var profileName = match.Groups["Profile"].Value;
                var chain = new CredentialProfileStoreChain();
                if (chain.TryGetAWSCredentials(profileName, out var awsCredentials))
                {
                    builder.Register(c => awsCredentials);
                }
            });
            route.MapRegex<EC2Handler>("ec2");
        });
    }

    protected override bool ItemExists(string path)
    {
        if (path.Contains("*"))
        {
            return false;
        }

        return WithPathHandler(path, handler => handler.Exists());
    }

    protected override void GetItem(string path)
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

    protected override void GetChildItems(string path, bool recurse)
    {
        WithPathHandler(path, handler =>
        {
            var childItems = handler.GetChildItems(useCache: false);
            WriteAwsItems(childItems);
        });
    }
    
    protected override bool HasChildItems(string path)
    {
        return WithPathHandler<bool?>(path, handler => handler.GetChildItems(useCache:true).Any()) ?? false;
    }

    protected override bool IsItemContainer(string path)
    {
        return WithPathHandler(path, handler => handler.GetItem()?.IsContainer) ?? false;
    }

    protected override void GetChildItems(string path, bool recurse, uint depth)
    {
        GetChildItems(path, recurse);
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

        try
        {
            var match = Router.Instance.Match(path);
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
}
