using System.Collections.ObjectModel;

namespace Application.Common.Models.ModemView;

public class ModemViewData
{
    public ModemViewData(string comPort)
    {
        ComPort = comPort;
    }
    public string Name { get; set; }
    public string Signal { get; set; }
    public string ComPort { get; }
    public ObservableCollection<string> ViewLogger { get; }

}
