using Amazon;
using Amazon.CertificateManager;
using Amazon.Runtime;
using Autofac;

namespace MountAws.Services.Acm;

public class AcmRegistrar : IServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AmazonCertificateManagerClient>().As<IAmazonCertificateManager>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}