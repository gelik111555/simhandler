namespace Application.Common.Interfaces;

public interface IComPort
{
    public IEnumerable<string> GetComPorts();
}
