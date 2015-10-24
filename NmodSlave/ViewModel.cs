using System.Collections.ObjectModel;
using System.Net;
using System.Threading;

namespace NmodSlave
{
    public class ViewModel : ObservableModel
    {
        private SynchronizationContext context;
        private NmodbusManager manager;

        public ViewModel()
        {
            ViewData = new ViewData() { MonitoredRegisters = new ObservableCollection<Register>() };
            Inputs = new UserInput();
            context = SynchronizationContext.Current;
        }

        public UserInput Inputs { get; set; }
        public ViewData ViewData { get; set; }

        public void AddAlias()
        {
            ViewData.MonitoredRegisters.Add(new Register() { Alias = Inputs.Alias, Address = Inputs.Register, Value = 0 });
        }

        public void Connect()
        {
            manager = new NmodbusManager(IPAddress.Parse(Inputs.IpAddress), (int)Inputs.Port, context);
            manager.StartListening();
            manager.ValueWasWritten += OnRegisterValueChanged;
        }

        public void Disconnect()
        {
            manager.ValueWasWritten -= OnRegisterValueChanged;
            manager.StopListening();
        }

        private Register FindElementInCollection(ObservableCollection<Register> haystack, uint addr)
        {
            var count = ViewData.MonitoredRegisters.Count;

            for (int i = 0; i < count; i++)
            {
                if (haystack[i].Address == addr)
                    return haystack[i];
            }

            return null;
        }

        private void OnRegisterValueChanged(object sender, Register modifiedRegister)
        {
            var item = FindElementInCollection(ViewData.MonitoredRegisters, modifiedRegister.Address);
            if (item != null) item.Value = modifiedRegister.Value;
        }
    }

    public class UserInput
    {
        public UserInput()
        {
            IpAddress = "127.0.0.1";
            Port = 502;
            Alias = string.Empty;
            Register = 0;
        }

        public string Alias { get; set; }
        public string IpAddress { get; set; }
        public uint Port { get; set; }
        public uint Register { get; set; }
    }

    public class ViewData
    {
        public ObservableCollection<Register> MonitoredRegisters { get; set; }
    }
}