using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Provider;
using Amazon.Runtime.CredentialManagement;
using Autofac;
using MountAws.Routing;
using MountAws.Services.EC2;

namespace MountAws;
[CmdletProvider("MountAws", ProviderCapabilities.None)]
public class MountAwsProvider : NavigationCmdletProvider, IPathHandlerContext
{
    private static readonly Cache _cache = new();

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
        Router.Instance.MapRegex<ProfileHandler>("(?<Profile>[a-z0-9-_]+)", route =>
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
        return GetPathHandler(path).Exists();
    }

    protected override void GetItem(string path)
    {
        var item = GetPathHandler(path).GetItem();
        if (item != null)
        {
            WriteAwsItem(item);
        }
    }

    protected override void GetChildItems(string path, bool recurse)
    {
        var items = GetPathHandler(path).GetChildItems(useCache: false);
        WriteAwsItems(items);
    }
    
    protected override bool HasChildItems(string path)
    {
        return GetPathHandler(path).GetChildItems(useCache:true).Any();
    }

    protected override bool IsItemContainer(string path)
    {
        return GetPathHandler(path).GetItem()?.IsContainer ?? false;
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

    private IPathHandler GetPathHandler(string path)
    {
        path = AwsPath.Normalize(path);
        if (string.IsNullOrEmpty(path))
        {
            return new ProfilesHandler(path, this);
        }
        
        
    }
    
    public string ToProviderPath(string path)
    {
        return $"{ItemSeparator}{path.Replace("/", ItemSeparator.ToString())}";
    }
}
