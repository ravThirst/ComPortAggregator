// DataAggregator.cs
using System;
using System.IO.Ports;
using System.Text;

namespace ComPortAggregator
{
    public class DataAggregator
    {
        private readonly SerialPort _virtualPort;
        private readonly object _lock = new object();

        public DataAggregator(string virtualPortName = "COM3", int baudRate = 9600)
        {
            _virtualPort = new SerialPort(virtualPortName, baudRate)
            {
                Encoding = Encoding.ASCII
            };
        }

        public void OpenVirtualPort()
        {
            if (!_virtualPort.IsOpen)
                _virtualPort.Open();
        }

        public void CloseVirtualPort()
        {
            if (_virtualPort.IsOpen)
                _virtualPort.Close();
        }

        public void ForwardData(string data)
        {
            lock (_lock)
            {
                if (_virtualPort.IsOpen)
                {
                    _virtualPort.Write(data);
                }
            }
        }
    }
}
