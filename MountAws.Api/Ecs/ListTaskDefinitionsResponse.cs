namespace MountAws.Api.Ecs;

public record ListTaskDefinitionsResponse(string[] TaskDefinitionArns, string NextToken) {}