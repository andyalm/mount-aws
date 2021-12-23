namespace MountAws.Services.Ecs;

public class CurrentCluster
{
    public CurrentCluster(string name)
    {
        Name = name;
    }
    
    public string Name { get; }
}