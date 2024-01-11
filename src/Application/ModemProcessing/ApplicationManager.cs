//using ModemLib.Services;
//using System.Collections.ObjectModel;

//namespace Application.ModemProcessing;

//public class ApplicationManager
//{
//    private readonly ObservableCollection<ModemViewModel> _modemViewModels;
//    private readonly AppDbContext _appDbContext;
//    private readonly ILogger _logger;

//    private readonly bool _isMockPortForTesting = false;
//    private readonly string[] _mockPorts = new string[1] { "COM50" };


//    public ApplicationManager
//        (ObservableCollection<ModemViewModel> modemViewModels, CancellationTokenSource cancellationTokenSource,
//        AppDbContext appDbContext, ILogger logger)
//    {
//        _modemViewModels = modemViewModels;
//        CancellationTokenSources = cancellationTokenSource;
//        _appDbContext = appDbContext;
//        _logger = logger;
//    }

//    public CancellationTokenSource CancellationTokenSources { get; private set; }

//    /// <summary>
//    /// Запускает обработку модемов.
//    /// </summary>
//    public async Task StartUp(bool isMock = false)
//    {
//        // Если это тестовый запуск
//        if (isMock)
//        {
//            // Выполняем тестовый запуск
//            await MockStartUp();
//            return;
//        }

//        try
//        {
//            // Получаем список доступных портов COM
//            IEnumerable<string> ports = ComPortService.GetAllPorts();

//            int portsCount = ports.ToList().Count;

//            if (portsCount == 0)
//            {
//                // Если нет доступных портов, записываем сообщение об ошибке в лог
//                _logger.Error("Отсутствуют порты {@int} на {Now} {Level:u3}", portsCount, DateTime.Now);
//                return;
//            }

//            List<Task> tasks = new();


//            foreach (var port in ports)
//            {
//                // Создаем задачу для обработки модема
//                tasks.Add(CreateNewTaskModemPooling(port));
//            }
//            // Запускаем все задачи параллельно
//            await Task.WhenAll(tasks);
//        }
//        catch (Exception ex)
//        {
//            // В случае исключения записываем сообщение об ошибке в лог и выбрасываем исключение
//            _logger.Fatal(ex, ex.Message, ex.StackTrace);
//            throw;
//        }
//    }
//    /// <summary>
//    /// Моковый запуск программы для работы в debug режиме
//    /// </summary>
//    /// <returns></returns>
//    private async Task MockStartUp()
//    {
//        try
//        {
//            List<Task> tasks = new();

//            foreach (var port in _mockPorts)
//            {
//                // Создаем задачу для обработки модема в тестовом режиме
//                tasks.Add(CreateNewTaskModemPooling(port));
//            }

//            // Запускаем все задачи параллельно
//            await Task.WhenAll(tasks);
//        }
//        catch (Exception ex)
//        {
//            // В случае исключения записываем сообщение об ошибке в лог и выбрасываем исключение
//            _logger.Fatal(ex, ex.Message, ex.StackTrace);
//            throw;
//        }
//    }

//    /// <summary>
//    /// Создает асинхронную задачу для опроса модема на указанном COM-порту.
//    /// </summary>
//    /// <param name="comPort">COM-порт, на котором находится модем.</param>
//    /// <returns>Асинхронную задачу, которая выполняет опрос модема.</returns>
//    private Task CreateNewTaskModemPooling(string comPort)
//    {
//        // Создаем модель view для отображения в таблице
//        var modemView = new ModemViewModel(comPort);
//        _modemViewModels.Add(modemView);
//        LoggerService loggerService = new(modemView, _logger);//Логгер для каждого потока свой

//        // Создаем модель для опроса модема, для каждого потока своя
//        var modemPollingService = new ModemPollingService(modemView, loggerService, _appDbContext, CancellationTokenSources);


//        // Возвращаем задачу, которая выполняет опрос модема с использованием токена отмены
//        return Task.Run(async () =>
//        {
//            await modemPollingService.PreparingToPollModem();
//        });

//    }
//}
