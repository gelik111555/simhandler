namespace Application.Common.Models.Settings.Operator;

public class OperatorSettings
{
    public string OperatorName { get; set; }
    public string GetPhoneNumberUSSD { get; set; }
    public string ActivationUSSD { get; set; }
    public bool GetPhoneWithUSSDCodeOrSMS { get; set; }
}
