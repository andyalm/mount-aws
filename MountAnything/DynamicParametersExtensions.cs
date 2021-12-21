using MountAnything.Routing;

namespace MountAnything;

internal static class DynamicParametersExtensions
{
    public static object? CreateDynamicParameters(this HandlerResolver handlerResolver, Type handlerParameterInterface)
    {
        var parameterInterface = handlerResolver.HandlerType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerParameterInterface);
        if (parameterInterface != null)
        {
            var parameterType = parameterInterface.GetGenericArguments().Single();
            return Activator.CreateInstance(parameterType);
        }

        return null;
    }

    public static void SetDynamicParameters(this IPathHandler handler, Type handlerParameterInterface, object? dynamicParameters)
    {
        var parameterInterface = handler.GetType().GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerParameterInterface);
        if (parameterInterface != null && dynamicParameters != null)
        {
            var parameterProperty = parameterInterface.GetProperties()
                .Single(p => p.CanWrite && dynamicParameters.GetType().IsAssignableFrom(p.PropertyType));
            
            parameterProperty.SetValue(handler, dynamicParameters);
        }
    }
}