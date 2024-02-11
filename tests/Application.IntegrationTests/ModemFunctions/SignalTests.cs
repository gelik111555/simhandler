using Application.Common.Exceptions;
using Infrastructure.Common.Interfaces;
using Infrastructure.ModemCommunication;
using Microsoft.Extensions.Logging;
using ModemLib.Domain.DTO;
using ModemLib.Domain.Interfaces;
using Moq;

namespace Infrastructure.IntegrationTests.ModemFunctions;

public class SignalTests
{
    [Test]
    public async Task GetSignalAsync_WhenMessageIsEmpty_ThrowsEmptyAnswerFromModemException()
    {
        // Arrange
        var modemMock = new Mock<IModem>();
        var modemBaseMock = new Mock<IModemBase>();
        var loggerMock = new Mock<ILogger>();
        var cancellationToken = new CancellationToken();

        string comPort = "COM103";
        string expectedResponse = ""; // Имитация ожидаемого ответа от модема
        var responseModem = new ResponseModem(expectedResponse);

        modemBaseMock.Setup(m => m.GetSignalLevelAsync(cancellationToken))
                     .ReturnsAsync(responseModem);

        modemMock.Setup(m => m.GetModemAsync(comPort, It.IsAny<ILogger>(), cancellationToken))
                 .ReturnsAsync(modemBaseMock.Object);

        var modemService = new ModemService(modemBaseMock.Object, new ModemSignal(), cancellationToken);

        Assert.ThrowsAsync<EmptyAnswerFromModemException>(async () => await modemService.GetSignalLevelAsync());
    }

    [Test]
    public async Task GetSignalAsync_WhenMessageIsOnlyOKWithOutDigit_ThrowsEmptyAnswerFromModemException()
    {
        // Arrange
        var modemMock = new Mock<IModem>();
        var modemBaseMock = new Mock<IModemBase>();
        var loggerMock = new Mock<ILogger>();
        var cancellationToken = new CancellationToken();

        string comPort = "COM103";
        string expectedResponse = "OK"; // Имитация ожидаемого ответа от модема
        var responseModem = new ResponseModem(expectedResponse);

        modemBaseMock.Setup(m => m.GetSignalLevelAsync(cancellationToken))
                     .ReturnsAsync(responseModem);

        modemMock.Setup(m => m.GetModemAsync(comPort, It.IsAny<ILogger>(), cancellationToken))
                 .ReturnsAsync(modemBaseMock.Object);

        var modemService = new ModemService(modemBaseMock.Object, new ModemSignal(), cancellationToken);

        Assert.ThrowsAsync<ModemResponseException>(async () => await modemService.GetSignalLevelAsync());
    }

    //[Test]
    //public async Task GetSignalAsync_ReturnsCorrectSignalLevel_WhenModemProvidesValidResponse()
    //{
    //    // Arrange
    //    var modemMock = new Mock<IModem>();
    //    var modemBaseMock = new Mock<IModemBase>();
    //    var loggerMock = new Mock<ILogger>();
    //    var cancellationToken = new CancellationToken();

    //    string comPort = "COM103";
    //    string expectedResponse = "+CSQ: 15,0\nOK"; // Имитация ожидаемого ответа от модема
    //    var responseModem = new ResponseModem(expectedResponse);

    //    modemBaseMock.Setup(m => m.GetSignalLevelAsync(cancellationToken))
    //                 .ReturnsAsync(responseModem);

    //    modemMock.Setup(m => m.GetModemAsync(comPort, It.IsAny<ILogger>(), cancellationToken))
    //             .ReturnsAsync(modemBaseMock.Object);

    //    var modemService = new ModemService(modemBaseMock.Object, new ModemSignal(), cancellationToken);

    //    // Act
    //    var result = await modemService.GetSignalLevelAsync();

    //    // Assert
    //    Assert.AreEqual("-83", result);
    //}

}

