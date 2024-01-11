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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="JsonReaderException"></exception>
    public async Task<OperatorSettings?> GetSettingsForOperator(string name)
    {
        if (!_fileSystem.File.Exists(_operatorSettingsJsonPath))
        {
            throw new FileNotFoundException($"Settings file not found at path: {_operatorSettingsJsonPath}");
        }

        string json = await _fileSystem.File.ReadAllTextAsync(_operatorSettingsJsonPath);

        List<OperatorSettings> settings = JsonConvert.DeserializeObject<List<OperatorSettings>>(json);

        // Выбор настроек для определенного оператора
        return settings.FirstOrDefault(s => s.OperatorName == name);
    }

    /// <summary>
    /// Устанавливает или обновляет настройки операторов. Этот метод проверяет каждую настройку на валидность,
    /// обновляет существующие настройки или добавляет новые, и сохраняет их в JSON-файл.
    /// </summary>
    /// <param name="operatorSettings">Коллекция настроек операторов для установки или обновления.</param>
    /// <returns>Task, представляющий асинхронную операцию.</returns>
    /// <exception cref="ValidationException">Выбрасывается, если какая-либо из предоставленных настроек не проходит валидацию.</exception>
    /// <exception cref="JsonReaderException">Выбрасывается, если происходит ошибка при десериализации существующего файла настроек. Может указывать на то, что настройки в файле имеют неверный формат.</exception>
    /// <exception cref="FileNotFoundException">Выбрасывается, если файл настроек не найден в ожидаемом расположении.</exception>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonContent"></param>
    /// <exception cref="JsonSerializationException"></exception>
    /// <returns></returns>
    private List<OperatorSettings> TryDeserializeOperatorSettings(string jsonContent)
    {
        try
        {
            return JsonConvert.DeserializeObject<List<OperatorSettings>>(jsonContent) ?? new List<OperatorSettings>();
        }
        catch (JsonSerializationException ex)
        {
            _logger.LogError(ex, "Произошла ошибка при десериализации JSON: {ErrorMessage} в файле operatorsettings.json", ex.Message);
            throw;
        }
        catch(JsonReaderException ex)
        {
            _logger.LogError(ex, "Произошла ошибка при чтения JSON: {ErrorMessage} в файле operatorsettings.json", ex.Message);
            throw;
        }
    }
}


