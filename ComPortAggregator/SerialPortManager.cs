// SerialPortManager.cs
using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace ComPortAggregator
{
    public class SerialPortManager
    {
        public HashSet<SerialPort> Ports { get; private set; }

        public void OpenPorts()
        {
            foreach (var port in Ports) {
                if (!port.IsOpen) port.Open();
            }
        }

        public void ClosePorts()
        {
            foreach (var port in Ports)
            {
                if (!port.IsOpen) port.Close();
            }
        }

        // SerialPortManager.cs (Updated)
        private readonly DataAggregator _aggregator;

        public SerialPortManager(List<string> ports, string virtualPortName = "COM11", int baudRate = 9600)
        {
            _aggregator = new DataAggregator(virtualPortName, baudRate);
            foreach (var port in ports)
            {
                var serialport = new SerialPort(port, baudRate);
                Ports.Add(serialport);
                serialport.DataReceived += (sender, args) => OnDataReceived(serialport);
            }
        }

        private void OnDataReceived(SerialPort port)
        {
            string data = port.ReadExisting();
            _aggregator.ForwardData(data);
        }

        public void OpenAllPorts()
        {
            OpenPorts();
            _aggregator.OpenVirtualPort();
        }

        public void CloseAllPorts()
        {
            ClosePorts();
            _aggregator.CloseVirtualPort();
        }

    }
}

