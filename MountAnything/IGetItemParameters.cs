namespace MountAnything;

public interface IGetItemParameters<T> where T : new()
{
    T GetItemParameters { set; }
}