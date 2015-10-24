namespace NmodSlave
{
    public class Register : ObservableModel
    {
        private ushort _value;
        public uint Address { get; set; }
        public string Alias { get; set; }

        public ushort Value
        {
            get { return _value; }
            set { SetValue(ref _value, value); }
        }
    }
}