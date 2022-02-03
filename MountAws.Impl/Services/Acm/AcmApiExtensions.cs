using Amazon.CertificateManager;
using Amazon.CertificateManager.Model;

namespace MountAws.Services.Acm;

public static class AcmApiExtensions
{
    public static CertificateDetail DescribeCertificate(this IAmazonCertificateManager acm, string certificateArn)
    {
        return acm.DescribeCertificateAsync(new DescribeCertificateRequest
        {
            CertificateArn = certificateArn
        }).GetAwaiter().GetResult().Certificate;
    }
}