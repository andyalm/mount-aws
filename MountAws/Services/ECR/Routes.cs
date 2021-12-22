using Autofac;
using MountAnything.Routing;

namespace MountAws.Services.ECR;

public static class Routes
{
    public static void MapECR(this Route route)
    {
        route.MapLiteral<ECRRootHandler>("ecr", ecr =>
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