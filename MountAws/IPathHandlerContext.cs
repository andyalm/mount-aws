namespace MountAws;

public interface IPathHandlerContext
{
    Cache Cache { get; }

    void WriteDebug(string message);
    bool Force { get; }
    string Filter { get; }
}