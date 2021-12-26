using System.Management.Automation;

namespace MountAnything;

public interface IPathHandlerContext
{
    Cache Cache { get; }

    void WriteDebug(string message);
    void WriteWarning(string message);
    bool Force { get; }
    CommandInvocationIntrinsics InvokeCommand { get; }
}