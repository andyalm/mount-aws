using MountAnything;

namespace MountAws.Services.Core;

public record CurrentProfile(string Value) : TypedString(Value);