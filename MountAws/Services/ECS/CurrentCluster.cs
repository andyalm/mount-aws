namespace MountAws.Services.ECS;

public class CurrentCluster
{
    public CurrentCluster(string name)
    {
        Name = name;
    }
    
    public string Name { get; }
}