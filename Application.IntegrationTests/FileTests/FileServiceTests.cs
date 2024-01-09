using Infrastructure.Files;
using Moq;
using Application.Common.Models.Settings.Operator;
using FluentValidation;
using FluentValidation.Results;
using System.IO.Abstractions;
using Microsoft.Extensions.Logging;

namespace Application.IntegrationTests.FileTests;
[TestFixture]
public class FileServiceTests
{
    private Mock<IFileSystem> _fileSystemMock;
    private Mock<IValidator<OperatorSettings>> _validatorMock;
    private Mock<ILogger<FileService>> _loggerMock;
    private FileService _fileService;

    [SetUp]
    public void SetUp()
    {
        _fileSystemMock = new Mock<IFileSystem>();
        _loggerMock = new Mock<ILogger<FileService>>();
        _validatorMock = new Mock<IValidator<OperatorSettings>>();
        _validatorMock.Setup(v => v.Validate(It.IsAny<OperatorSettings>()))
                      .Returns(new ValidationResult());
        _fileService = new FileService(_fileSystemMock.Object, _validatorMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task SetOrUpdateSettingsForOperator_ValidSettings_ShouldSaveToFile()
    {
        // Arrange
        var settings = new List<OperatorSettings>
            {
                new OperatorSettings { /* ... инициализация свойств ... */ }
            };

        // Создаем мок для Stream.
        var streamMock = new Mock<Stream>();

        // Мок IFileSystem.
        var fileSystemMock = new Mock<IFileSystem>();

        // Настраиваем fileSystemMock, чтобы метод Create возвращал мокнутый объект Stream.
        fileSystemMock
            .Setup(fs => fs.FileStream.Create(It.IsAny<string>(), FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            
                      .Returns(streamMock.Object);

        // Инициализация FileService с мокнутыми зависимостями.
        _fileService = new FileService(fileSystemMock.Object, _validatorMock.Object, _loggerMock.Object);

        // Act
        await _fileService.SetOrUpdateSettingsForOperator(settings);

        // Assert
        _fileSystemMock.Verify(fs => fs.File.Open(It.IsAny<string>(), FileMode.OpenOrCreate, FileAccess.Write, FileShare.None), Times.Once);
    }

}
