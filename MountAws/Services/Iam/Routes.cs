using Autofac;
using Microsoft.Extensions.DependencyInjection;
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
                    policy.ConfigureServices((services, match) =>
                    {
                        services.AddSingleton(IamItemPath.Parse(match.Values["PolicyPathAndName"]));
                    });
                });
            });
            iam.MapLiteral<RolesHandler>("roles", roles =>
            {
                roles.MapRegex<RoleHandler>(@"(?<RolePathAndName>[a-z0-9+=,.@\-/]+)", role =>
                {
                    role.ConfigureServices((services, match) =>
                    {
                        services.AddSingleton(IamItemPath.Parse(match.Values["RolePathAndName"]));
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
            iam.MapLiteral<UsersHandler>("users", users =>
            {
                users.MapRegex<UserHandler>(@"(?<UserPathAndName>[a-z0-9+=,.@\-/]+)", user =>
                {
                    user.ConfigureServices((services, match) =>
                    {
                        services.AddSingleton(IamItemPath.Parse(match.Values["UserPathAndName"]));
                    });
                    user.MapLiteral<UserPoliciesHandler>("policies", rolePolicies =>
                    {
                        rolePolicies.Map<UserPolicyHandler>();
                    });
                    user.MapLiteral<UserStatementsHandler>("statements", statements =>
                    {
                        statements.Map<UserStatementHandler>();
                    });
                });
            });
        });
    }
}