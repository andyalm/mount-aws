using System.Management.Automation.Provider;

namespace MountAnything.Content;

public interface IContentReaderHandler
{
    IContentReader GetContentReader();
}