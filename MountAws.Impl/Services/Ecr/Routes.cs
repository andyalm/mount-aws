using Autofac;
using MountAnything.Routing;

namespace MountAws.Services.Ecr;

public static class Routes
{
    public static void MapEcr(this Route route)
    {
        route.MapLiteral<EcrRootHandler>("ecr", ecr =>
        {
            ecr.MapRegex<RepositoryHandler>(@"(?<RepositoryPath>.+)", repository =>
            {
                repository.RegisterServices((match, builder) =>
                {
                    builder.RegisterInstance(new RepositoryPath(match.Values["RepositoryPath"]));
                });
                repository.MapLiteral<ImageTagsHandler>("image-tags", imageTags =>
                {
                    imageTags.Map<ImageTagHandler>();
                });
            });
        });
    }
}