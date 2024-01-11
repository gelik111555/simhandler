using FluentValidation;

namespace Application.Common.Models.Settings.Operator;


public class OperatorSettingsValidator : AbstractValidator<OperatorSettings>
{
    public OperatorSettingsValidator()
    {
        RuleFor(os => os.OperatorName)
            .MinimumLength(3).WithMessage("Имя оператора должно содержать минимум 3 символа.")
            .MaximumLength(15).WithMessage("Имя оператора должно содержать максимум 15 символов.");

        RuleFor(os => os.GetPhoneNumberUSSD)
            .NotEmpty().WithMessage("USSD-код для получения номера телефона не может быть пустым.")
            .Matches(@"^\*\d{4,12}#$").WithMessage("USSD-код для получения номера телефона должен начинаться с '*', заканчиваться на '#' и содержать от 4 до 12 цифр.")
            .Length(2, 10).WithMessage("USSD-код для получения номера телефона должен содержать от 2 до 10 символов.");

        RuleFor(os => os.ActivationUSSD)
            .NotEmpty().When(os => !string.IsNullOrEmpty(os.ActivationUSSD))
            .WithMessage("USSD-код активации не может быть пустым.")
            .Length(6, 15).When(os => !string.IsNullOrEmpty(os.ActivationUSSD))
            .WithMessage("USSD-код активации должен содержать от 6 до 15 символов.");
    }
}
