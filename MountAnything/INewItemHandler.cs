namespace MountAnything;

public interface INewItemHandler
{
    void NewItem(string itemTypeName, object? newItemValue);
}