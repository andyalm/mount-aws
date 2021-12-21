using System.Text;

namespace MountAnything.Content;

public class StringContentReader : StreamContentReader
{
    private readonly string _content;

    public StringContentReader(string content)
    {
        _content = content;
    }

    protected override MemoryStream CreateContentStream()
    {
        var bytes = Encoding.UTF8.GetBytes(_content);
        return new MemoryStream(bytes);
    }
}