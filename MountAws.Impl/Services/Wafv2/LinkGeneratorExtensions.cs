using MountAnything;

namespace MountAws.Services.Wafv2;

public static class LinkGeneratorExtensions
{
    public static ItemPath Wafv2CloudfrontWebAcl(this LinkGenerator linkGenerator, string webAclArn)
    {
        var aclName = webAclArn.Split("/")[^2];
        return linkGenerator.Wafv2CloudfrontPath().Combine("cloudfront-web-acls", aclName);
    }
    
    private static ItemPath Wafv2CloudfrontPath(this LinkGenerator linkGenerator)
    {
        return linkGenerator.ConstructPath(1, "us-east-1/wafv2");
    }
}