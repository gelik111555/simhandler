using Application.Common.Models;
using Application.Common.Models.ModemView;
using System.Collections.ObjectModel;

namespace Application.Common.Interfaces;

public interface IModemViewDataManager
{
    void AddComPortsView(IEnumerable<string> comPorts);
    ModemViewData GetComPortView(string comPort);
    ObservableCollection<ModemViewData> GetAll();
}
