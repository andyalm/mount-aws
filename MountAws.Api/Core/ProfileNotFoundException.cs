namespace MountAws.Api;

public class ProfileNotFoundException : ApplicationException
{
    public ProfileNotFoundException(string profileName) : base($"The profile '{profileName}' does not exist")
    {
        
    }
}