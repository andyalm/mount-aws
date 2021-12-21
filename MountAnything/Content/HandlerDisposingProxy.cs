using System.Collections;
using System.Management.Automation.Provider;
using Autofac;

namespace MountAnything.Content;

public class HandlerDisposingProxy : IContentReader, IContentWriter
{
    private readonly ILifetimeScope _lifetimeScope;
    private readonly IContentReader? _reader;
    private readonly IContentWriter? _writer;

    public HandlerDisposingProxy(ILifetimeScope lifetimeScope, IContentReader reader)
    {
        _lifetimeScope = lifetimeScope;
        _reader = reader;
    }
    public HandlerDisposingProxy(ILifetimeScope lifetimeScope, IContentWriter writer)
    {
        _lifetimeScope = lifetimeScope;
        _writer = writer;
    }

    public void Dispose()
    {
        try
        {
            _reader?.Close();
        }
        finally
        {
            _lifetimeScope.Dispose();
        }
    }

    public void Close()
    {
        _reader?.Close();
        _writer?.Close();
    }


    public void Seek(long offset, SeekOrigin origin)
    {
        _reader?.Seek(offset, origin);
        _writer?.Seek(offset, origin);
    }
    
    public IList Read(long readCount) => _reader!.Read(readCount);

    public IList Write(IList content) => _writer!.Write(content);
}