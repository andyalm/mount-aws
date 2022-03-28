using Amazon.ECR.Model;

namespace MountAws.Services.Ecr;

public static class ModelExtensions
{
    public static VulnerablePackage? GetVulnerablePackage(this ImageScanFinding finding)
    {
        var packageName = finding.Attributes?.FirstOrDefault(a => a.Key == "package_name")?.Value;
        var packageVersion = finding.Attributes?.FirstOrDefault(a => a.Key == "package_version")?.Value;

        if (!string.IsNullOrEmpty(packageName) && !string.IsNullOrEmpty(packageVersion))
        {
            return new VulnerablePackage(packageName, packageVersion);
        }

        return null;
    }

    public static VulnerablePackage[] GetVulnerablePackages(this EnhancedImageScanFinding finding)
    {
        return finding.PackageVulnerabilityDetails?.VulnerablePackages
                   .Select(p => new VulnerablePackage(p.Name, p.Version)).ToArray() ??
               Array.Empty<VulnerablePackage>();
    }
}

public record VulnerablePackage(string Name, string Version)
{
    public override string ToString()
    {
        return $"{Name}[{Version}]";
    }
}