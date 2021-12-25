using System.Collections.Immutable;
using System.Management.Automation;

namespace MountAnything;

public interface IItem
{
    string ParentPath { get; }
    string FullPath { get; }
    string ItemName { get; }
    string ItemType { get; }
    bool IsContainer { get; }
    string TypeName { get; }
    IEnumerable<string> CacheablePaths { get; }
    ImmutableDictionary<string, Item> Links { get; }
    PSObject ToPipelineObject(Func<string,string> pathResolver);
}