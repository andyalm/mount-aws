namespace MountAws;

public class WebUrlBuilder
{
    public static implicit operator string(WebUrlBuilder builder) => builder.ToString();
    
    public static WebUrlBuilder Regionless()
    {
        return new WebUrlBuilder("https://console.aws.amazon.com");
    }
    
    public static WebUrlBuilder S3()
    {
        return new WebUrlBuilder("https://s3.console.aws.amazon.com");
    }
    
    public static WebUrlBuilder ForRegion(string regionName)
    {
        return new WebUrlBuilder($"https://{regionName}.console.aws.amazon.com");
    }
    
    private readonly string _value;
    
    private WebUrlBuilder(string value)
    {
        _value = value;
    }

    public WebUrlBuilder CombineWith(string path)
    {
        return new WebUrlBuilder($"{_value}/{path}");
    }

    public override string ToString()
    {
        return _value;
    }
}