namespace MountAnything;

public interface IRemoveItemParameters<in T> where T : new()
{
    T RemoveItemParameters { set; }
}