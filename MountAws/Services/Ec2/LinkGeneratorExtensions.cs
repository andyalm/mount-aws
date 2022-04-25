using Amazon.EC2.Model;
using MountAnything;

namespace MountAws.Services.Ec2;

public static class LinkGeneratorExtensions
{
    private static ItemPath Ec2ServicePath(this LinkGenerator linkGenerator)
    {
        return linkGenerator.ConstructPath(2, "ec2");
    }
    public static InstanceItem Ec2Instance(this LinkGenerator linkGenerator, Instance instance, Image? image)
    {
        var ec2ServicePath = linkGenerator.Ec2ServicePath();
        var parentPath = ec2ServicePath.Combine("instances");

        return new InstanceItem(parentPath, instance, image, linkGenerator);
    }

    public static ItemPath AutoScalingGroup(this LinkGenerator linkGenerator, string asgName)
    {
        return linkGenerator.Ec2ServicePath().Combine("auto-scaling-groups", asgName);
    }

    public static ItemPath Ec2Image(this LinkGenerator linkGenerator, string imageId)
    {
        return linkGenerator.Ec2ServicePath().Combine("images", imageId);
    }
    
    public static ImageItem Ec2Image(this LinkGenerator linkGenerator, Image image)
    {
        var ec2ServicePath = linkGenerator.Ec2ServicePath();
        var parentPath = ec2ServicePath.Combine("images");
        
        return new ImageItem(parentPath, image);
    }
}