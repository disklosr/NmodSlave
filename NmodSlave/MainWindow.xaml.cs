using System.Windows;

namespace NmodSlave
{
    public delegate void NModbusWrittenValue(object source, Register modifiedRegister);

    public partial class MainWindow : Window
    {
        private ViewModel vm;

        public MainWindow()
        {
            vm = new ViewModel();
            DataContext = vm;
            InitializeComponent();
        }

        private void AddReg_Click(object sender, RoutedEventArgs e)
        {
            vm.AddAlias();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (Connect.Content.ToString() == "Connect")
            {
                vm.Connect();
                Connect.Content = "Disconnect";
                InputIp.IsEnabled = InputPort.IsEnabled = false;
            }
            else
            {
                vm.Disconnect();
                Connect.Content = "Connect";
                InputIp.IsEnabled = InputPort.IsEnabled = true;
            }
        }
    }
}