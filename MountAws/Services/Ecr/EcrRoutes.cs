using Autofac;
using Microsoft.Extensions.DependencyInjection;
using MountAnything.Routing;

namespace MountAws.Services.Ecr;

public class EcrRoutes : IServiceRoutes
{
    public void AddServiceRoutes(Route route)
    {
        route.MapLiteral<EcrRootHandler>("ecr", ecr =>
        {
            ecr.MapRegex<RepositoryHandler>(@"(?<RepositoryPath>.+)", repository =>
            {
                repository.ConfigureServices((services, match) =>
                {
                    services.AddSingleton(new RepositoryPath(match.Values["RepositoryPath"]));
                });
                repository.MapLiteral<ImageTagsHandler>("image-tags", imageTags =>
                {
                    imageTags.Map<ImageTagHandler>(imageTag =>
                    {
                        imageTag.MapLiteral<ImageScanHandler>("image-scan", imageScan =>
                        {
                            imageScan.Map<ImageScanFindingHandler>();
                        });
                    });
                });
            });
        });
    }
}