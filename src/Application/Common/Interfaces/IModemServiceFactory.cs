namespace Application.Common.Interfaces;

public interface IModemServiceFactory
{
    public Task<IModemService> CreateAsync(string comPort);
}
