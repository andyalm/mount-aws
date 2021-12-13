namespace MountAnything;

public interface IGetChildItemParameters
{
    object GetChildItemParameters { get; set; }
}

public interface IGetChildItemParameters<T> : IGetChildItemParameters where T : notnull
{
    object IGetChildItemParameters.GetChildItemParameters
    {
        get => GetChildItemParameters;
        set => GetChildItemParameters = (T)value;
    }

    new T GetChildItemParameters { get; set; }
}