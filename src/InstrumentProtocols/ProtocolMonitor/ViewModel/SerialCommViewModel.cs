using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using NLog;
using Prover.Client.Framework.Screens;
using Prover.InstrumentProtocol.Core.Factories;
using Prover.InstrumentProtocol.Core.IO;
using Prover.InstrumentProtocol.Core.Models.Instrument;
using Prover.InstrumentProtocol.Honeywell.Factories;
using Prover.ProtocolMonitor.Model;
using ReactiveUI;
using RJCP.IO.Ports;

namespace Prover.ProtocolMonitor.ViewModel
{
    public class SerialCommViewModel : ViewModelBase
    {
        #region Private Fields

        private static readonly string AppTitle = "SerialComm Monitor V2";
        
        private ICommand _changeFileLocation;
        private ICommand _clear;
        private ICommand _exportTxtFile;
        private string _fileName = "output_data";
        private bool _isAutoscrollChecked = true;
        private ICommand _open;
        private ICommand _openLink;

        private string _receivedSerialData;
        private IDisposable _receivedSerialDataObservable;
        private ICommand _refreshPorts;

        private string _sentSerialData;
        private IDisposable _sentSerialDataObservable;
        private DispatcherTimer _timer;

        public Subject<string> CommWindowSubject { get; } = new Subject<string>();

        #endregion

        #region  Constructors

        public SerialCommViewModel()
        {
            Logger.Log(LogLevel.Info, "--- PROGRAM STARTED ---");
            Logger.Log(LogLevel.Debug, "New instance of SerialPort() is initialized.");

            GetSettings();
            SetDefaults();

            var canConnect = this.WhenAnyValue(x => x.SelectedCommPort, x => x.SelectedInstrument, x => x.IsConnected,
                (port, instr, connected) => port != null && instr != null && IsConnected == false);
            
            ConnectCommand = ReactiveCommand.CreateFromTask(StartListening, canConnect);

            var canDisconnect = this.WhenAnyValue(x => x.IsConnected).Select(c => c);
            DisconnectCommand = ReactiveCommand.CreateFromTask(StopListening, canDisconnect);

            var canDownload = this.WhenAnyValue(x => x.IsConnected);
            _downloadCommand = ReactiveCommand.CreateFromTask<string>(DownloadItems, canDownload);

            CommWindowSubject
                .Subscribe(s => SerialCommData += s + Environment.NewLine);

            Logger.Log(LogLevel.Debug, "All default values are set. End of SerialCommViewModel() constructor!");
        }

        #endregion

        #region Public Properties

        private bool _isConnected;

        public bool IsConnected
        {
            get { return _isConnected; }
            set { this.RaiseAndSetIfChanged(ref _isConnected, value); }
        }


        private ReactiveCommand<string, Unit> _downloadCommand;

        public ReactiveCommand<string, Unit> DownloadCommand
        {
            get { return _downloadCommand; }
            set { this.RaiseAndSetIfChanged(ref _downloadCommand, value); }
        }

        public static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public List<SerialPortSettingsModel> BaudRates { get; private set; }

        public ICommand ChangeFileLocation
        {
            get
            {
                _changeFileLocation = new RelayCommand(
                    param => ChangeFileLocationMethod());
                return _changeFileLocation;
            }
        }

        public ICommand Clear
        {
            get
            {
                _clear = new RelayCommand(
                    param => SerialCommData = "");
                return _clear;
            }
        }

        public ReactiveCommand DisconnectCommand { get; set; }
        public List<SerialPortSettingsModel> CommPorts { get; private set; }

        private IInstrument _connectedInstrument;

        public IInstrument ConnectedInstrument
        {
            get { return _connectedInstrument; }
            set { this.RaiseAndSetIfChanged(ref _connectedInstrument, value); }
        }

        
        public int[] DataBits { get; set; }
        public bool EnableDisableSettings { get; set; }

        public ICommand ExportFile
        {
            get
            {
                _exportTxtFile = new RelayCommand(
                    param => ExportFileMethod(),
                    param => ExportFileCanExecute());
                return _exportTxtFile;
            }
        }

        public string ExportStatus { get; set; }
        public bool ExportStatusSuccess { get; set; }
        public string[] FileExtensions { get; set; }
        public string FileLocation { get; set; }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
         
            }
        }

        private List<IInstrumentFactory> _instruments;
        public List<IInstrumentFactory> Instruments
        {
            get { return _instruments; }
            set { this.RaiseAndSetIfChanged(ref _instruments, value); }
        }

        public bool IsAutoscrollChecked
        {
            get { return _isAutoscrollChecked; }
            set
            {
                _isAutoscrollChecked = value;
                if (_isAutoscrollChecked)
                    ScrollOnTextChanged = true;
                else
                    ScrollOnTextChanged = false;

          
            }
        }

        public bool IsDTR { get; set; }
        public bool IsRTS { get; set; }
        public List<SerialPortSettingsModel> LineEndings { get; private set; }

        private ReactiveCommand _connectCommand;

        public ReactiveCommand ConnectCommand
        {
            get { return _connectCommand; }
            set { this.RaiseAndSetIfChanged(ref _connectCommand, value); }
        }

        public ICommand OpenLink
        {
            get
            {
                _openLink = new RelayCommand(
                    param => Process.Start("https://github.com/heiswayi/SerialComm"));
                return _openLink;
            }
        }

        public List<SerialPortSettingsModel> Parities { get; private set; }

        public ICommand RefreshPorts
        {
            get
            {
                _refreshPorts = new RelayCommand(param => RefreshPortsMethod());
                return _refreshPorts;
            }
        }

        // Disable interaction from UI
        // TODO: Trigger TextBoxAutomaticScrollingExtension.cs or Scroll to
        //       end the TextBox when CheckBox is checked for second time.
        public string ScrollConfirm
        {
            get
            {
                // Debug only
                //return "Autoscroll (" + ScrollOnTextChanged.ToString() + ")";
                return "Autoscroll";
            }
        }

        public bool ScrollOnTextChanged { get; private set; }
        public int SelectedBaudRate { get; set; }

        private SerialPortSettingsModel _selectedCommPort;
        public SerialPortSettingsModel SelectedCommPort
        {
            get { return _selectedCommPort; }
            set { this.RaiseAndSetIfChanged(ref _selectedCommPort, value); }
        }

        public int SelectedDataBits { get; set; }
        public string SelectedFileExtension { get; set; }

        private IInstrumentFactory _selectedInstrument;
        public IInstrumentFactory SelectedInstrument
        {
            get { return _selectedInstrument; }
            set { this.RaiseAndSetIfChanged(ref _selectedInstrument, value); }
        }

        public string SelectedLineEnding { get; set; }
        public Parity SelectedParity { get; set; }
        public StopBits SelectedStopBits { get; set; }

        public string SerialCommData
        {
            get { return _receivedSerialData; }
            set { this.RaiseAndSetIfChanged(ref _receivedSerialData, value); }
        }

        public List<SerialPortSettingsModel> StopBitsList { get; private set; }


        public string WindowTitle { get; private set; }

        #endregion

        #region  Public Methods

        /// <summary>
        ///     Close port if port is open when user closes MainWindow.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            try
            {
                if (ConnectedInstrument != null && ConnectedInstrument.CommunicationClient.IsConnected)
                {
                    ConnectedInstrument.CommunicationClient.Dispose();

                    Logger.Log(LogLevel.Debug,
                        "SerialPort.Dispose() & SerialPort.Close() are executed on OnWindowClosing() method.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex);
            }
        }

        #endregion

        private void ChangeFileLocationMethod()
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "File Location";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = FileLocation;

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = FileLocation;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folder = dlg.FileName;
                // Do something with selected folder string
                FileLocation = folder;
            }
        }

        private bool ExportFileCanExecute()
        {
            return FileName != "";
        }

        private void ExportFileMethod()
        {
            try
            {
                if (File.Exists(FileLocation + @"\" + FileName + SelectedFileExtension))
                {
                    var msgBoxResult = MessageBox.Show(
                        "File " + FileName + SelectedFileExtension +
                        " already exists!\n Select 'Yes' to overwrite the existing file or\n'No' to create a new file with timestamp suffix or\n 'Cancel' to cancel?",
                        "Overwrite Confirmation",
                        MessageBoxButton.YesNoCancel);
                    if (msgBoxResult == MessageBoxResult.Yes)
                    {
                        File.WriteAllText(FileLocation + @"\" + FileName + SelectedFileExtension, SerialCommData);
                        Logger.Log(LogLevel.Debug,
                            "Output data is saved and exported into " + FileLocation + @"\" + FileName +
                            SelectedFileExtension);

                        ExportStatus = "Done.";
                        ExportStatusSuccess = true;
                        StartTimer(10);
                    }
                    else if (msgBoxResult == MessageBoxResult.No)
                    {
                        File.WriteAllText(
                            FileLocation + @"\" + FileName + DateTime.Now.ToString("-yyyyMMddHHmmss") +
                            SelectedFileExtension, SerialCommData);
                        Logger.Log(LogLevel.Debug,
                            "Output data is saved and exported into " + FileLocation + @"\" + FileName +
                            DateTime.Now.ToString("-yyyyMMddHHmmss") + SelectedFileExtension);
                        ExportStatus = "Done.";
                       
                        ExportStatusSuccess = true;
                        
                        StartTimer(10);
                    }
                }
                else
                {
                    File.WriteAllText(FileLocation + @"\" + FileName + SelectedFileExtension, SerialCommData);
                    Logger.Log(LogLevel.Debug,
                        "Output data is saved and exported into " + FileLocation + @"\" + FileName +
                        SelectedFileExtension);
                    ExportStatus = "Done.";
                    
                    ExportStatusSuccess = true;
           
                    StartTimer(10);
                }
            }
            catch (Exception ex)
            {
                ExportStatus = "Error exporting a file!";
                
                ExportStatusSuccess = false;
          
                StartTimer(10);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex);
            }
        }

        private List<IInstrumentFactory> FindInstrumentFactories()
        {
            var commFunc =
                new Func<ICommPort>(() =>
                    new SerialCommPort(SelectedCommPort.DeviceId, SelectedBaudRate, dataBits: SelectedDataBits,
                        parity: SelectedParity, timeoutMs: 250, stopBits: SelectedStopBits));
            return new List<IInstrumentFactory>
            {
                new MiniAtFactory(commFunc),
                new MiniMaxFactory(commFunc)
            };
        }

        /// <summary>
        ///     Get connection/communication status.
        /// </summary>
        /// <returns>String of Connected/Disconnect</returns>
        private string GetConnectionStatus()
        {
            if (ConnectedInstrument?.CommunicationClient != null && ConnectedInstrument.CommunicationClient.IsConnected)
                return "Connected";
            return "Not Connected";
        }

        private void GetSettings()
        {
            // Get lists of settings objects
            try
            {
                CommPorts = SerialPortSettingsModel.Instance.GetCommPorts();
                BaudRates = SerialPortSettingsModel.Instance.GetBaudRates();
                Parities = SerialPortSettingsModel.Instance.GetParities();
                DataBits = SerialPortSettingsModel.Instance.GetDataBits;
                StopBitsList = SerialPortSettingsModel.Instance.GetStopBits();
                LineEndings = SerialPortSettingsModel.Instance.GetLineEndings();
                FileExtensions = FileExportSettingsModel.Instance.getFileExtensions;
                Logger.Log(LogLevel.Debug, "All lists of settings objects are loaded.");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex);
            }
        }

        /// <summary>
        ///     Rescan avaiable ports
        /// </summary>
        private void RefreshPortsMethod()
        {
            try
            {
                CommPorts = SerialPortSettingsModel.Instance.GetCommPorts();
      
                SelectedCommPort = CommPorts.FirstOrDefault();
      
                Logger.Log(LogLevel.Debug, "New list of COM* ports are repopulated.");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex);
            }
        }

        private void SetDataListeners()
        {
            ConnectedInstrument.CommunicationClient.ResponseStream.ObserveOnDispatcher()
                .Select(r => $"R => {r}{Environment.NewLine}")
                .Subscribe(CommWindowSubject);
            ConnectedInstrument.CommunicationClient.SentStream.ObserveOnDispatcher()
                .Select(s => $"W => {s}")
                .Subscribe(CommWindowSubject);
        }

        private void SetDefaults()
        {
// Set default values
            if (CommPorts != null) SelectedCommPort = CommPorts.FirstOrDefault();
            SelectedBaudRate = 9600;
            SelectedParity = Parity.None;
            SelectedDataBits = 8;
            SelectedStopBits = StopBits.One;
            SelectedLineEnding = "";
            IsDTR = true;
            IsRTS = true;
            FileLocation = AssemblyDirectory;
            SelectedFileExtension = FileExtensions[0];
            WindowTitle = AppTitle + " (" + GetConnectionStatus() + ")";
            EnableDisableSettings = true;
            ScrollOnTextChanged = true;
            Instruments = FindInstrumentFactories();
        }

        /// <summary>
        ///     Initiate serial port communication.
        /// </summary>
        private async Task StartListening()
        {
            try
            {
                if (SelectedInstrument != null)
                {
                    ConnectedInstrument = SelectedInstrument.Create();
                    SetDataListeners();
                    var stopWatch = new Stopwatch();
                    stopWatch.Restart();

                    ConnectionCancellationToken = new CancellationTokenSource();
                    await ConnectedInstrument.CommunicationClient.Connect(ConnectedInstrument, ConnectionCancellationToken.Token);
                    IsConnected = true;
                    stopWatch.Stop();

                    EnableDisableSettings = false;
                    

                    CommWindowSubject.OnNext(
                        $"Connected to {ConnectedInstrument.Name}: {SelectedCommPort.DeviceId}, {SelectedBaudRate} baud, Parity.{SelectedParity}, {SelectedDataBits}, StopBits.{SelectedStopBits}, RTS={IsRTS}, DTR={IsDTR}");

                    Logger.Log(LogLevel.Debug, "Ready to receive data...");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex);
            }
            
        }

        public CancellationTokenSource ConnectionCancellationToken { get; set; }

        private async Task DownloadItems(string itemsType)
        {
            if (ConnectedInstrument?.CommunicationClient != null)
                switch (itemsType)
                {
                    case "All":
                        await ConnectedInstrument.GetAllItems();
                        break;
                    case "Temperature":
                        await ConnectedInstrument.GetTemperatureItems();
                        break;
                    case "Pressure":
                        await ConnectedInstrument.GetPressureItems();
                        break;
                    case "Volume":
                        await ConnectedInstrument.GetVolumeItems();
                        break;
                }
        }

        /// <summary>
        ///     Allow/disallow StartListening() to be executed.
        /// </summary>
        /// <returns>True/False</returns>
        private bool StartListeningCanExecute()
        {
            if (CommPorts == null)
                return false;
            if (ConnectedInstrument == null || !ConnectedInstrument.CommunicationClient.IsConnected)
                return true;
            return false;
        }

        private void StartTimer(int duration)
        {
            if (_timer != null)
            {
                _timer.Stop();
                ExportStatus = "";
              
            }
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(duration);
            _timer.Tick += TimerTick;
            _timer.Start();
        }

        /// <summary>
        ///     Terminate serial port communication.
        /// </summary>
        private async Task StopListening()
        {
            if (ConnectedInstrument?.CommunicationClient != null)
                try
                {
                    ConnectionCancellationToken.Cancel();
                    CommWindowSubject.OnNext($"Disconnected from {ConnectedInstrument?.Name}");
                    ConnectedInstrument?.CommunicationClient?.Dispose();
                    IsConnected = false;
                    ConnectedInstrument = null;

                    EnableDisableSettings = true;
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex);
                }
        }

        private void TimerTick(object send, EventArgs e)
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
                ExportStatus = "";
               
            }
        }
    }
}