namespace Application.Common.Interfaces;

public interface IModemService
{
    public string GetName();
    public Task<string> GetSignalLevelAsync();
}
