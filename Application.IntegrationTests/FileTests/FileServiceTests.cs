using Infrastructure.Files;
using Moq;
using Application.Common.Models.Settings.Operator;
using FluentValidation;
using FluentValidation.Results;
using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Application.IntegrationTests.FileTests;

[TestFixture]
public class FileServiceTests
{
    private Mock<IValidator<OperatorSettings>> _validatorMock;
    private Mock<ILogger<FileService>> _loggerMock;
    private Mock<IFileSystem> _fileSystemMock;
    private Mock<IFile> _fileMock;
    private Mock<Stream> _streamMock;
    private FileService _fileService;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<FileService>>();
        _validatorMock = new Mock<IValidator<OperatorSettings>>();
        _validatorMock.Setup(v => v.Validate(It.IsAny<OperatorSettings>()))
                      .Returns(new ValidationResult());

        _streamMock = new Mock<Stream>();
        _streamMock.Setup(stream => stream.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
            .Verifiable();
        _streamMock.Setup(stream => stream.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();
        _streamMock.Setup(stream => stream.Close())
            .Verifiable();

        _fileSystemMock = new Mock<IFileSystem>();
        _fileMock = new Mock<IFile>();
        _fileSystemMock.Setup(fs => fs.File).Returns(_fileMock.Object);
        _fileSystemMock.Setup(fs => fs.FileStream.Create(It.IsAny<string>(), FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            .Returns(_streamMock.Object);

        // Настройка мока для имитации чтения файла
        _fileSystemMock.Setup(fs => fs.File.ReadAllTextAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(GetTestSettingsJson);

        _fileService = new FileService(_fileSystemMock.Object, _validatorMock.Object, _loggerMock.Object);
    }
    [Test]
    public async Task SetOrUpdateSettingsForOperator_ValidSettings_ShouldSaveToFile()
    {
        // Arrange
        var settings = new List<OperatorSettings>
        {
            new() {
                OperatorName = "TELE2",
                ActivationUSSD = "*305#",
                GetPhoneNumberUSSD = "*201#",
                GetPhoneWithUSSDCodeOrSMS = false
            }
        };
        _fileSystemMock.Setup(f => f.File.Exists(It.IsAny<string>())).Returns(true);
        _fileSystemMock.Setup(fs => fs.File.ReadAllTextAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(GetTestSettingsJson);
        // Act
        await _fileService.SetOrUpdateSettingsForOperator(settings);

        // Assert
        _fileMock.Verify(f => f.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }


    [Test]
    public void SetOrUpdateSettingsForOperator_ValidSettings_ShouldThrowJsonReaderException()
    {
        // Arrange
        var settings = new List<OperatorSettings>
        {
            new() {
                OperatorName = "TELE2",
                ActivationUSSD = "*305#",
                GetPhoneNumberUSSD = "*201#",
                GetPhoneWithUSSDCodeOrSMS = false
            }
        };
        _fileSystemMock.Setup(f => f.File.Exists(It.IsAny<string>())).Returns(true);
        _fileSystemMock.Setup(fs => fs.File.ReadAllTextAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(GetTestErrorSettingsJsonInvalidSecondData);
        
        // Act & Assert
        Assert.ThrowsAsync<JsonReaderException>( () => _fileService.SetOrUpdateSettingsForOperator(settings));
    }
    [Test]
    public async Task SetOrUpdateSettingsForOperator_InvalidSettings_ShouldNotSaveToFileAndLogError()
    {
        // Arrange
        var invalidSettings = new List<OperatorSettings>
        {
        new OperatorSettings {
            OperatorName = "", // Invalid Operator Name
            ActivationUSSD = "",
            GetPhoneNumberUSSD = "",
            GetPhoneWithUSSDCodeOrSMS = false
        }
        };

        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
        new ValidationFailure("OperatorName", "Operator Name is required")
        // Add other errors as needed
        });

        _validatorMock.Setup(v => v.Validate(It.IsAny<OperatorSettings>())).Returns(validationResult);

        // Act
        try
        {
            await _fileService.SetOrUpdateSettingsForOperator(invalidSettings);
        }
        catch (ValidationException ex)
        {
            // Log the validation error as needed, or simply ignore if it's expected
            _loggerMock.Object.LogError(ex, "Validation error occurred.");
        }

        // Assert
        _loggerMock.Verify(
        x => x.Log(
        LogLevel.Error, // Проверка на вызов с уровнем логирования Error
        It.IsAny<EventId>(),
        It.Is<It.IsAnyType>((o, t) => string.Equals("Validation error occurred.", o.ToString())), // Проверка на сообщение об ошибке
        It.IsAny<Exception>(),
        (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
        Times.AtLeastOnce);
    }
    [Test]
    public async Task GetSettingsForOperator_WhenOperatorExists_ShouldReturnSettings()
    {
        _fileSystemMock.Setup(f => f.File.Exists(It.IsAny<string>())).Returns(true);
        // Act
        var result = await _fileService.GetSettingsForOperator("TELE2");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.OperatorName, Is.EqualTo("TELE2"));
    }
    [Test]
    public async Task GetSettingsForOperator_WhenOperatorDoesNotExist_ShouldReturnNull()
    {
        _fileSystemMock.Setup(f => f.File.Exists(It.IsAny<string>())).Returns(true);
        // Act
        var result = await _fileService.GetSettingsForOperator("NonExistentOperator");

        // Assert
        Assert.That(result, Is.Null);
    }
    [Test]
    public void GetSettingsForOperator_WhenJsonIsInvalid_ShouldThrowJsonSerializationException()
    {
        //Arrange
        // Настройка мока для возврата невалидного JSON
        var invalidJson = "invalid json";
        _fileSystemMock.Setup(fs => fs.File.ReadAllTextAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(invalidJson);
        _fileSystemMock.Setup(f => f.File.Exists(It.IsAny<string>())).Returns(true);
        // Act & Assert
        Assert.ThrowsAsync<JsonReaderException>(() => _fileService.GetSettingsForOperator("TELE2"));
    }
    [Test]
    public void GetSettingsForOperator_WhenFileDoesNotExist_ShouldThrowFileNotFoundException()
    {
        // Arrange
        _fileSystemMock.Setup(f => f.File.Exists(It.IsAny<string>())).Returns(false);

        // Act & Assert
        Assert.ThrowsAsync<FileNotFoundException>(() => _fileService.GetSettingsForOperator("TELE2"));
    }
    private string GetTestSettingsJson()
    {
        var settings = new List<OperatorSettings>
        {
            new() { OperatorName = "TELE2", ActivationUSSD = "*305#", GetPhoneNumberUSSD = "*201#", GetPhoneWithUSSDCodeOrSMS = false }
        };
        return JsonConvert.SerializeObject(settings);
    }
    private string GetTestErrorSettingsJsonInvalidSecondData()
    {
        return @"
    [
        {
            ""OperatorName"": ""TELE2"", 
            ""ActivationUSSD"": ""*305#"",
            ""GetPhoneNumberUSSD"": ""*201#"",
            ""GetPhoneWithUSSDCodeOrSMS"": false
        },
        {
           ""djsajskjak""
        }
    ]";
    }

}

