using Autofac;
using MountAnything.Routing;

namespace MountAws.Services.Iam;

public class Routes : IServiceRoutes
{
    public void AddServiceRoutes(Route region)
    {
        region.MapLiteral<IamRootHandler>("iam", iam =>
        {
            iam.MapLiteral<PoliciesHandler>("policies", policies =>
            {
                policies.MapRegex<PolicyHandler>(@"(?<PolicyPathAndName>[a-z0-9+=,.@\-/]+)", policy =>
                {
                    policy.RegisterServices((match, builder) =>
                    {
                        builder.RegisterInstance(IamItemPath.Parse(match.Values["PolicyPathAndName"]));
                    });
                });
            });
        });
    }
}