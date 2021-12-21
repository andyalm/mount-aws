using System.Management.Automation.Provider;

namespace MountAnything.Content;

public interface IContentWriterHandler
{
    IContentWriter GetContentWriter();
}