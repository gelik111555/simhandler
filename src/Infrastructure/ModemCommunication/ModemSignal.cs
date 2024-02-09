using Application.Common.Exceptions;
using Infrastructure.Common.Interfaces;
using ModemLib.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace Infrastructure.ModemCommunication;

internal partial class ModemSignal : IModemSignal
{
    private string _responseModem;
    private const string _errorEmptyMessage = "Message with signal level was empty";
    private const string _errorResponseMessage = "It is not possible to get numbers from the modem's response to determine the signal strength";
    private const string _regularExpressionForSearchingForNumbers = @"\b\d{2},\d{2}\b";
    private float _signalQuality;

    public async Task<string> GetSignalAsync(IModemBase modem, CancellationToken cancellationToken)
    {
        var message = await modem.GetSignalLevelAsync(cancellationToken);
        if (message.MessageIsEmpty) 
            throw new EmptyAnswerFromModemException(_errorEmptyMessage);
        _responseModem = message.Message;
        return ExtractSignalLevelInDecibels();
    }

    private string ExtractSignalLevelInDecibels()
    {
        GetOnlyDigits();
        return ConversionToDecibelsFormat();
    }

    [GeneratedRegex("\\+CSQ: (\\d+,\\d+)")]
    private static partial Regex SearchingForOnlyNumbers();
    private void GetOnlyDigits()
    {
        var match = SearchingForOnlyNumbers().Match(_responseModem);
        if (!match.Success)
            throw new ModemResponseException(_errorResponseMessage);
        _signalQuality = float.Parse(match.Groups[1].Value);
    }

    private string ConversionToDecibelsFormat()
    {
        return (_signalQuality * 2 - 113).ToString();
    }

    
}
