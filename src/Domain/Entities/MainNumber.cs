using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class MainNumber : BaseEntity
{
    public string PhoneNumber { get; set; }
    public string ICCID { get; set; }
    public NumberStatus Status { get; set; }
    public DateTime RequestTime { get; set; } = DateTime.Now;
    public List<AdditionalNumber> AdditionalNumbers { get; set; } = new List<AdditionalNumber>();
}
