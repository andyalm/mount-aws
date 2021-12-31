namespace MountAnything;

public interface INewItemParameters<T> where T : new()
{
    T NewItemParameters { set; }
}