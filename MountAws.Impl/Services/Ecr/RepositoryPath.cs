using MountAnything;

namespace MountAws.Services.Ecr;

public record RepositoryPath(string Value) : TypedString(Value);