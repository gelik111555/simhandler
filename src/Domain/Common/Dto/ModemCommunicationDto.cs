namespace Domain.Common.Dto;

public class ModemCommunicationDto
{
    public string ComPort { get; }
    public CancellationToken CancellationToken { get;}

    public ModemCommunicationDto(string comPort, CancellationToken cancellationToken)
    {
        ComPort = comPort;
        CancellationToken = cancellationToken;
    }
}
