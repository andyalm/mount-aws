using System.Management.Automation;
using MountAnything;
using MountAws.Services.Ec2;

namespace MountAws;

public static class LinkGeneratorExtensions
{
    public static InstanceItem EC2Instance(this LinkGenerator linkGenerator, PSObject instance)
    {
        var ec2ServicePath = linkGenerator.EC2ServicePath();
        var parentPath = ItemPath.Combine(ec2ServicePath, "instances");

        return new InstanceItem(parentPath, instance);
    }

    private static string EC2ServicePath(this LinkGenerator linkGenerator)
    {
        return linkGenerator.ConstructPath(2, "ec2");
    }
}