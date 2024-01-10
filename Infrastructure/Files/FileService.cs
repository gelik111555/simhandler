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
        // Проверка существования файла настроек
        if (!_fileSystem.File.Exists(_operatorSettingsJsonPath))
        {
            // Можно выбросить исключение или вернуть пустую коллекцию
            throw new FileNotFoundException("Settings file not found.");
        }

        // Чтение файла JSON
        string json = await _fileSystem.File.ReadAllTextAsync(_operatorSettingsJsonPath);

        // Десериализация JSON в список настроек
        List<OperatorSettings> settings = JsonConvert.DeserializeObject<List<OperatorSettings>>(json);

        // Выбор настроек для определенного оператора
        return settings.FirstOrDefault(s => s.OperatorName == name);
    }

    public async Task SetOrUpdateSettingsForOperator(ICollection<OperatorSettings> operatorSettings)
    {
        foreach (var setting in operatorSettings)
        {
            var validationResult = _validator.Validate(setting);
            if (!validationResult.IsValid)
            {
                // Обработка ошибок валидации
                throw new ValidationException(validationResult.Errors);
            }
        }

        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var filePath = Path.Combine(baseDir, _operatorSettingsJsonPath);
        List<OperatorSettings> existingSettings = new();
        if (_fileSystem.File.Exists(filePath))
        {
            var fileContent = _fileSystem.File.ReadAllText(filePath);
            try
            {
                existingSettings = JsonConvert.DeserializeObject<List<OperatorSettings>>(fileContent) ?? new List<OperatorSettings>();
            }
            catch (JsonSerializationException ex)
            {
                _logger.LogError(ex, "Произошла ошибка при DeserializeObject JSON: {ErrorMessage} в файле operatorsettings.json", ex.Message);
                //throw;
            }
        }
        else
        {
            _logger.LogError("Файл {FilePath} не найден.", filePath);
            throw new FileNotFoundException($"Файл настроек {filePath} не найден.");
        }

        // Update existing settings or add new ones
        foreach (var setting in operatorSettings)
        {
            var existingSetting = existingSettings.FirstOrDefault(s => s.OperatorName == setting.OperatorName);
            if (existingSetting != null)
            {
                // Update existing setting
                existingSetting.GetPhoneNumberUSSD = setting.GetPhoneNumberUSSD;
                existingSetting.ActivationUSSD = setting.ActivationUSSD;
                existingSetting.GetPhoneWithUSSDCodeOrSMS = setting.GetPhoneWithUSSDCodeOrSMS;
            }
            else
            {
                // Add new setting
                existingSettings.Add(setting);
            }
        }

        // Write the updated settings to the file using IFileSystem abstraction
        await _fileSystem.File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(existingSettings, Formatting.Indented));

    }
}


