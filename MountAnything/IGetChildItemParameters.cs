namespace MountAnything;

public interface IGetChildItemParameters<in T> where T : new()
{
    T GetChildItemParameters { set; }
}