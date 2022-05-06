using Amazon.ECR;
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

    public static void MergeSeverityCounts(this ImageScanFindings findings, ImageScanFindings otherFindings)
    {
        MergeSeverityCounts(FindingSeverity.CRITICAL, findings.FindingSeverityCounts, otherFindings.FindingSeverityCounts);
        MergeSeverityCounts(FindingSeverity.HIGH, findings.FindingSeverityCounts, otherFindings.FindingSeverityCounts);
        MergeSeverityCounts(FindingSeverity.MEDIUM, findings.FindingSeverityCounts, otherFindings.FindingSeverityCounts);
        MergeSeverityCounts(FindingSeverity.LOW, findings.FindingSeverityCounts, otherFindings.FindingSeverityCounts);
        MergeSeverityCounts(FindingSeverity.INFORMATIONAL, findings.FindingSeverityCounts, otherFindings.FindingSeverityCounts);
        MergeSeverityCounts(FindingSeverity.UNDEFINED, findings.FindingSeverityCounts, otherFindings.FindingSeverityCounts);
    }

    private static void MergeSeverityCounts(FindingSeverity severity, Dictionary<string, int> findings,
        Dictionary<string, int> moreFindings)
    {
        var value1 = findings.GetValueOrDefault(severity.Value, 0);
        var value2 = moreFindings.GetValueOrDefault(severity.Value, 0);

        findings[severity.Value] = value1 + value2;
    }
}

public record VulnerablePackage(string Name, string Version)
{
    public override string ToString()
    {
        return $"{Name}[{Version}]";
    }
}