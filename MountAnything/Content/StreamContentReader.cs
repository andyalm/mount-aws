using System.Collections;
using System.Management.Automation.Provider;

namespace MountAnything.Content;

public abstract class StreamContentReader : IContentReader
{
    private Lazy<Stream> _contentStream;

    protected StreamContentReader()
    {
        _contentStream = new Lazy<Stream>(CreateContentStream);
    }

    protected abstract Stream CreateContentStream();

    public void Dispose()
    {
        Close();
    }

    public void Close()
    {
        if (_contentStream.IsValueCreated)
        {
            _contentStream.Value.Close();
            _contentStream = new Lazy<Stream>(CreateContentStream);
        }
    }

    public IList Read(long readCount)
    {
        var blocks = new List<string>();

        var reader = new StreamReader(_contentStream.Value);
        // It is observed that displaying content can be slow especially on xterm cases.
        // Thus by default, we read them all and return all once. This means the user cannot use
        // Get-Content -TotalCount feature, which is fine comparing the speed of displaying content output.
        var content = reader.ReadToEnd();
        if (content.Length > 0)
        {
            // For some reason ReadToEnd() or Readline() inserts LF at the end. So trim them off here.
            blocks.Add(content.TrimEnd(Environment.NewLine.ToCharArray()));
        }

        return blocks;
    }

    public void Seek(long offset, SeekOrigin origin)
    {
        _contentStream.Value.Seek(offset, origin);
    }
}