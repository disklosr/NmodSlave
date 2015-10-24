using Modbus.Data;
using Modbus.Device;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NmodSlave
{
    public class NmodbusManager
    {
        private IPAddress addr;
        private SynchronizationContext context;
        private Register modifiedRegister;
        private int port;

        private ModbusSlave slave;

        private TcpListener slaveTcpListener;

        public NmodbusManager(IPAddress addr, int port, SynchronizationContext context)
        {
            this.addr = addr;
            this.port = port;
            this.context = context;
        }

        public event NModbusWrittenValue ValueWasWritten;

        public void StartListening()
        {
            slaveTcpListener = new TcpListener(addr, port);
            slaveTcpListener.Start();
            slave = ModbusTcpSlave.CreateTcp(1, slaveTcpListener);

            slave.DataStore = DataStoreFactory.CreateDefaultDataStore();
            slave.DataStore.DataStoreWrittenTo += DataStore_DataStoreWrittenTo;

            slave.Listen();
        }

        public void StopListening()
        {
            slave.Dispose();
            slaveTcpListener.Stop();
        }

        private void DataStore_DataStoreWrittenTo(object sender, DataStoreEventArgs e)
        {
            if (e.ModbusDataType != ModbusDataType.HoldingRegister) return;

            var data = (DataStore)sender;
            var newValue = data.HoldingRegisters[e.StartAddress + 1];

            modifiedRegister = new Register()
            {
                Address = e.StartAddress,
                Value = newValue
            };

            Debugger.Log(1, "Debug", modifiedRegister.Value + " | " + newValue);
            context.Post((a) => ValueWasWritten?.Invoke(null, modifiedRegister), null);
        }
    }
}