using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tobii_TestTask.Core;
using Tobii_TestTask.WPF;

namespace Tobii_TestTask
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void OutputControl_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            OutputScroll.ScrollToBottom();
        }

        private void TextBoxControl_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }
    }

    public class MainWindowViewModel
    {
        private readonly TaskManager _taskRunner;
        public MainWindowModel Model { get; }
        private ICommand _createTasksCommand;

        public ICommand CreateTasksCommand
        {
            get { return _createTasksCommand ?? (_createTasksCommand = new RelayCommand(param => RunNewSimulation())); }
        }

        public MainWindowViewModel()
        {
            _taskRunner = new TaskManager();
            _taskRunner.Run();
            Model = new MainWindowModel { CountOfClients = 10, CountOfTasks = 10 };
        }

        private void RunNewSimulation()
        {
            Model.Output = string.Empty;

            for (int clientIndex = 0; clientIndex < Model.CountOfClients; clientIndex++)
            {
                new TaskFactory().StartNew(clientIndexPar =>
                {
                    Random rand = new Random();
                    for (int taskIndex = 0; taskIndex < Model.CountOfTasks; taskIndex++)
                    {
                        Thread.Sleep(rand.Next(0, 100)); //small delay for simulation time spent for creation new task
                        var index = taskIndex;
                        _taskRunner.Add(a =>
                        {
                            Thread.Sleep(rand.Next(0, 100)); //small delay for simulation work
                            Model.Output +=
                                $"{DateTime.Now:yyyy-MM-dd HH-mm-ss} TaskID={a.Id:D4} from client #{clientIndexPar:D3}, task #{index:D3}.{Environment.NewLine}";
                        });
                    }
                }, clientIndex);
            }
        }
    }

    public class MainWindowModel : ModelHelper_NotifyPropertyChanged
    {
        private int _countOfClients;
        private int _countOfTasks;
        private string _output;

        public int CountOfClients
        {
            get => _countOfClients;
            set
            {
                _countOfClients = value;
                NotifyPropertyChanged();
            }
        }

        public int CountOfTasks
        {
            get => _countOfTasks;
            set
            {
                _countOfTasks = value;
                NotifyPropertyChanged();
            }
        }

        public string Output
        {
            get => _output;
            set
            {
                _output = value;
                NotifyPropertyChanged();
            }
        }
    }
}