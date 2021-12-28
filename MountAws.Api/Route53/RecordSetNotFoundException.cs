namespace MountAws.Api.Route53;

public class RecordSetNotFoundException : ApplicationException
{
    public RecordSetNotFoundException(string recordIdentifier) : base($"Record set '{recordIdentifier}' does not exist")
    {
        
    }
}