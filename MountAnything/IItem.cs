using System.Collections.Immutable;
using System.Management.Automation;

namespace MountAnything;

public interface IItem
{
    ItemPath ParentPath { get; }
    ItemPath FullPath { get; }
    string ItemName { get; }
    string ItemType { get; }
    bool IsContainer { get; }
    string TypeName { get; }
    IEnumerable<ItemPath> CacheablePaths { get; }
    ImmutableDictionary<string, IItem> Links { get; }
    ImmutableDictionary<string, ItemPath> LinkPaths { get; }
    PSObject ToPipelineObject(Func<ItemPath,string> pathResolver);
}