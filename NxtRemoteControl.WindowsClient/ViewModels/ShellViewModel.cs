using System.IO.Ports;
using Caliburn.Micro;

namespace NxtRemoteControl.WindowsClient.ViewModels
{
    public class ShellViewModel : Screen
    {
        private readonly IWindowManager _window;
        private readonly IEventAggregator _events;
        private string _portName;

        public ShellViewModel(IWindowManager window, IEventAggregator events)
        {
            _window = window;
            _events = events;

            DisplayName = "NXT Remote Control";
        }

        public string PortName
        {
            get { return _portName; }
            set { _portName = value; NotifyOfPropertyChange(() => PortName);}
        }

        public string[] PortNames
        {
            get { return SerialPort.GetPortNames(); }
        }

        
        public void Connect()
        {
              
        }
    }
}