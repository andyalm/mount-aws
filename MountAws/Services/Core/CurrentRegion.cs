using MountAnything;

namespace MountAws.Services.Core;

public record CurrentRegion(string Value) : TypedString(Value);