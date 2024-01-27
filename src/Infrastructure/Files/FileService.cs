using Application.Common.Interfaces;
using Application.Common.Models.Settings.Operator;
using FluentValidation;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.IO.Abstractions;

namespace Infrastructure.Files;

public class FileService : ISettingsService
{
    private readonly IFileSystem _fileSystem;
    private readonly IValidator<OperatorSettings> _validator;
    private readonly ILogger<FileService> _logger;
    const string _operatorSettingsJsonPath = @"Files\Configurations\operatorsettings.json";
    public FileService(IValidator<OperatorSettings> validator, ILogger<FileService> logger)
        : this(null, validator, logger) { }
    public FileService(IFileSystem fileSystem, IValidator<OperatorSettings> validator, ILogger<FileService> logger)
    {
        _fileSystem = fileSystem ?? new FileSystem();
        _validator = validator;
        _logger = logger;
    }
    public event EventHandler OperatorSettingsChanged;
    
    public async Task<OperatorSettings?> GetSettingsForOperator(string name)
    {
        if (!_fileSystem.File.Exists(_operatorSettingsJsonPath))
        {
            throw new FileNotFoundException($"Settings file not found at path: {_operatorSettingsJsonPath}");
        }

        string json = await _fileSystem.File.ReadAllTextAsync(_operatorSettingsJsonPath);

        List<OperatorSettings> settings = JsonConvert.DeserializeObject<List<OperatorSettings>>(json);

        return settings.FirstOrDefault(s => s.OperatorName == name);
    }

    public async Task CreateConfigurationFileIfNotExist()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _operatorSettingsJsonPath);

        if (!_fileSystem.File.Exists(filePath))
        {
            _logger.LogInformation($"Файл настроек не найден. Создание нового файла по пути: {filePath}");
            await _fileSystem.File.WriteAllTextAsync(filePath, "[]");
        }
    }

    public async Task ClearConfigurationFile()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _operatorSettingsJsonPath);

        if (!_fileSystem.File.Exists(filePath))
        {
            throw new FileNotFoundException($"Файл настроек не найден по пути: {filePath}");
        }

        await _fileSystem.File.WriteAllTextAsync(filePath, "[]");
    }
    
    public async Task SetOrUpdateSettingsForOperator(ICollection<OperatorSettings> operatorSettings)
    {
        foreach (var setting in operatorSettings)
        {
            var validationResult = _validator.Validate(setting);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }

        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var filePath = Path.Combine(baseDir, _operatorSettingsJsonPath);
        List<OperatorSettings> existingSettings = new();
        if (_fileSystem.File.Exists(filePath))
        {
            var fileContent = await _fileSystem.File.ReadAllTextAsync(filePath);
            try
            {
                existingSettings = TryDeserializeOperatorSettings(fileContent);
            }
            catch (JsonSerializationException){}
            catch (JsonReaderException)
            {
                throw;
            }
        }
        else
        {
            _logger.LogError("Файл {FilePath} не найден.", filePath);
            throw new FileNotFoundException($"Файл настроек {filePath} не найден.");
        }

        foreach (var setting in operatorSettings)
        {
            var existingSetting = existingSettings.FirstOrDefault(s => s.OperatorName == setting.OperatorName);
            if (existingSetting != null)
            {
                existingSetting.GetPhoneNumberUSSD = setting.GetPhoneNumberUSSD;
                existingSetting.ActivationUSSD = setting.ActivationUSSD;
                existingSetting.GetPhoneWithUSSDCodeOrSMS = setting.GetPhoneWithUSSDCodeOrSMS;
            }
            else
            {
                existingSettings.Add(setting);
            }
        }

        await _fileSystem.File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(existingSettings, Formatting.Indented));

    }

    private List<OperatorSettings> TryDeserializeOperatorSettings(string jsonContent)
    {
        try
        {
            return JsonConvert.DeserializeObject<List<OperatorSettings>>(jsonContent) ?? new List<OperatorSettings>();
        }
        catch (JsonSerializationException ex)
        {
            _logger.LogError(ex, "Произошла ошибка при дессериализации JSON: {ErrorMessage} в файле operatorsettings.json", ex.Message);
            throw;
        }
        catch(JsonReaderException ex)
        {
            _logger.LogError(ex, "Произошла ошибка при чтения JSON: {ErrorMessage} в файле operatorsettings.json", ex.Message);
            throw;
        }
    }
}


