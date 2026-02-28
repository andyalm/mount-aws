using Microsoft.Extensions.DependencyInjection;
using MountAnything.Routing;

namespace MountAws.Services.SecretsManager;

public class Routes : IServiceRoutes
{
    public void AddServiceRoutes(Route regionRoute)
    {
        regionRoute.MapLiteral<SecretsManagerRootHandler>("secretsmanager", secretsManager =>
        {
            secretsManager.MapLiteral<SecretsHandler>("secrets", secrets =>
            {
                secrets.MapRegex<SecretHandler>(@"(?<SecretPath>.+)", secret =>
                {
                    secret.ConfigureServices((services, match) =>
                    {
                        services.AddSingleton(new SecretPath(match.Values["SecretPath"]));
                    });
                });
            });
        });
    }
}
