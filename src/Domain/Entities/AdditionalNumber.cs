using Domain.Common;
using Domain.Entities;

public class AdditionalNumber : BaseEntity
{
    public string PhoneNumber { get; set; }
    public double EarnedAmount { get; set; }
    public string MainPhoneNumber { get; set; }
    public MainNumber MainNumber { get; set; }
}
