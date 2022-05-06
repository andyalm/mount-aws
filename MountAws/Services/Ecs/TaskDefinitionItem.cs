using System.Management.Automation;
using Amazon.ECS.Model;
using MountAnything;

namespace MountAws.Services.Ecs;

public class TaskDefinitionItem : AwsItem
{
    public TaskDefinitionItem(ItemPath parentPath, TaskDefinition taskDefinition, Tag[] tags) : base(parentPath, new PSObject(taskDefinition))
    {
        var taskFamilyAndRevision = taskDefinition.TaskDefinitionArn.Split("/").Last();
        var parts = taskFamilyAndRevision.Split(":");
        Family = parts[0];
        ItemName = parts[1];
        UnderlyingObject.Properties.Add(new PSNoteProperty("Tags", tags));
    }
    
    public TaskDefinitionItem(ItemPath parentPath, string taskDefinitionArn) : base(parentPath, new PSObject())
    {
        var taskFamilyAndRevision = taskDefinitionArn.Split("/").Last();
        var parts = taskFamilyAndRevision.Split(":");
        Family = parts[0];
        ItemName = parts[1];
    }

    public string Family { get; }
    public override string ItemName { get; }
    public override bool IsContainer => false;

    protected override void CustomizePSObject(PSObject psObject)
    {
        base.CustomizePSObject(psObject);
        psObject.Properties.Add(new PSNoteProperty("Family", Family));
        psObject.Properties.Add(new PSNoteProperty("Revision", ItemName));
        psObject.Properties.Add(new PSNoteProperty("TaskDefinitionName", $"{Family}:{ItemName}"));
    }
    public override string? WebUrl => UrlBuilder.CombineWith($"ecs/home#/taskDefinitions/{Family}/{ItemName}");
}