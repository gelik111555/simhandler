using Application.Common.Interfaces;
using System.Threading.Tasks;
using System;
using System.Windows;

namespace WPF;

public partial class MainWindow : Window
{
    private readonly IMainDataGridHandler _mainDataGridHandler;

    public MainWindow(IMainDataGridHandler mainDataGridHandler)
    {
        InitializeComponent();
        _mainDataGridHandler = mainDataGridHandler;
        LoadDataAsync();
    }

    private async void LoadDataAsync()
    {
        try
        {
            await Task.Run(() =>
            {
                _mainDataGridHandler.Handle();
            });
        }
        catch (Exception ex)
        {
            // Обработка исключений
            Dispatcher.Invoke(() =>
            {
                // Показать сообщение об ошибке или логировать
            });
        }
    }
}
