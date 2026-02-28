using System;
using AwesomeAssertions;
using MountAnything;
using MountAnything.Routing;
using MountAws.Services.SecretsManager;
using Xunit;

namespace MountAws.UnitTests;

public class SecretsManagerTests
{
    private readonly Router _router;

    public SecretsManagerTests()
    {
        var provider = new MountAwsProvider();
        _router = provider.CreateRouter();
    }

    [Theory]
    [InlineData("myprofile/us-east-1/secretsmanager", typeof(SecretsManagerRootHandler))]
    [InlineData("myprofile/us-east-1/secretsmanager/secrets", typeof(SecretsHandler))]
    [InlineData("myprofile/us-east-1/secretsmanager/secrets/my-secret", typeof(SecretHandler))]
    [InlineData("myprofile/us-east-1/secretsmanager/secrets/my-secret/password", typeof(SecretValueHandler))]
    public void SecretsManagerRoutesResolveToCorrectHandlers(string path, Type expectedHandlerType)
    {
        var resolver = _router.GetResolver(new ItemPath(path));
        resolver.HandlerType.Should().Be(expectedHandlerType);
    }
}
