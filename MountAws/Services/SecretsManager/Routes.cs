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
                secrets.Map<SecretHandler>(secret =>
                {
                    secret.Map<SecretValueHandler>();
                });
            });
        });
    }
}
