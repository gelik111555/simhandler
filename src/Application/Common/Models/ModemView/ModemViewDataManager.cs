using Application.Common.Interfaces;
using System.Collections.ObjectModel;

namespace Application.Common.Models.ModemView;

public class ModemViewDataManager : IModemViewDataManager
{
    private readonly Dictionary<string, ModemViewData> _comPortViews = new();

    public void AddComPortsView(IEnumerable<string> comPorts)
    {
        foreach (var item in comPorts)
        {
            _comPortViews[item] = new ModemViewData(item);
        }
    }

    public ModemViewData GetComPortView(string comPort)
    {
        if (_comPortViews.TryGetValue(comPort, out ModemViewData view))
        {
            return view;
        }

        throw new KeyNotFoundException($"View for COM port '{comPort}' not found.");
    }

    public ObservableCollection<ModemViewData> GetAll()
    {
        return new ObservableCollection<ModemViewData>(_comPortViews.Values);
    }
}
