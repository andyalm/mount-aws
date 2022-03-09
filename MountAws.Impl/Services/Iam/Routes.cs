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
            iam.MapLiteral<RolesHandler>("roles", roles =>
            {
                roles.MapRegex<RoleHandler>(@"(?<RolePathAndName>[a-z0-9+=,.@\-/]+)", role =>
                {
                    role.RegisterServices((match, builder) =>
                    {
                        builder.RegisterInstance(IamItemPath.Parse(match.Values["RolePathAndName"]));
                    });
                    role.MapLiteral<RolePoliciesHandler>("policies", rolePolicies =>
                    {
                        rolePolicies.Map<RolePolicyHandler>();
                    });
                    role.MapLiteral<RoleStatementsHandler>("statements", statements =>
                    {
                        statements.Map<RoleStatementHandler>();
                    });
                });
            });
        });
    }
}