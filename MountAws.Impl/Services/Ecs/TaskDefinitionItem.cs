using System.Management.Automation;

namespace MountAws.Services.Ecs;

public class TaskDefinitionItem : AwsItem
{
    public TaskDefinitionItem(string parentPath, PSObject taskDefinition) : base(parentPath, taskDefinition)
    {
        var taskFamilyAndRevision = Property<string>("TaskDefinitionArn")!.Split("/").Last();
        var parts = taskFamilyAndRevision.Split(":");
        Family = parts[0];
        ItemName = parts[1];
    }
    
    public TaskDefinitionItem(string parentPath, string taskDefinitionArn) : base(parentPath, new PSObject())
    {
        var taskFamilyAndRevision = taskDefinitionArn.Split("/").Last();
        var parts = taskFamilyAndRevision.Split(":");
        Family = parts[0];
        ItemName = parts[1];
    }

    public string Family { get; }
    public override string ItemName { get; }
    public override bool IsContainer => false;
    public override void CustomizePSObject(PSObject psObject)
    {
        base.CustomizePSObject(psObject);
        psObject.Properties.Add(new PSNoteProperty("Family", Family));
        psObject.Properties.Add(new PSNoteProperty("Revision", ItemName));
        psObject.Properties.Add(new PSNoteProperty("TaskDefinitionName", $"{Family}:{ItemName}"));
    }
}