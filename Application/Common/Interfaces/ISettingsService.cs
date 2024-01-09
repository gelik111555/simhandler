﻿using Application.Common.Models.Settings.Operator;

namespace Application.Common.Interfaces;

public interface ISettingsService
{
    public event EventHandler OperatorSettingsChanged;
    public Task<OperatorSettings> GetSettingsForOperator(string name);
    public Task SetOrUpdateSettingsForOperator(ICollection<OperatorSettings> operatorSettings);
}
