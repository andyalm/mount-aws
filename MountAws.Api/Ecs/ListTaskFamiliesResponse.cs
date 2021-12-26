namespace MountAws.Api.Ecs;

public record ListTaskFamiliesResponse(string[] Families, string NextToken);