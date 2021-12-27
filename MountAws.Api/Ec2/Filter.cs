namespace MountAws.Api.Ec2;

public class Filter
{
    public string Name { get; }
    public List<string> Values { get; }

    public Filter(string name, string value)
    {
        Name = name;
        Values = new List<string>{value};
    }

    public Filter(string name, IEnumerable<string> values)
    {
        Name = name;
        Values = values.ToList();
    }
}