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

    public FileService(IValidator<OperatorSettings> validator, ILogger<FileService> logger)
        : this(null, validator, logger) { }
    public FileService(IFileSystem fileSystem, IValidator<OperatorSettings> validator, ILogger<FileService> logger)
    {
        _fileSystem = fileSystem ?? new FileSystem();
        _validator = validator;
        _logger = logger;
    }
    public event EventHandler OperatorSettingsChanged;

    public Task<OperatorSettings> GetSettingsForOperator(string name)
    {
        throw new NotImplementedException();
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
        var filePath = Path.Combine(baseDir, "Files\\Configurations\\operatorsettings.json");
        List<OperatorSettings> existingSettings = new();
        if (File.Exists(filePath))
        {
            var fileContent = File.ReadAllText(filePath);
            try
            {
                existingSettings = JsonConvert.DeserializeObject<List<OperatorSettings>>(fileContent) ?? new List<OperatorSettings>();
            }
            catch (JsonSerializationException ex)
            {
                _logger.LogError(ex, "Произошла ошибка при десериализации JSON: {ErrorMessage} в файле operatorsettings.json", ex.Message);
            }
        }
        //// Read existing settings if the file exists
        //var existingSettings = File.Exists(filePath)
        //    ? JsonConvert.DeserializeObject<List<OperatorSettings>>(File.ReadAllText(filePath))
        //    : new List<OperatorSettings>();

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
