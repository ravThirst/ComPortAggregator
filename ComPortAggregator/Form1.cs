using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComPortAggregator
{
    public partial class Form1 : Form
    {
        private SerialPortManager _serialPortManager;

        public Form1()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            this.Text = "Virtual COM Port Aggregator";
            this.ClientSize = new System.Drawing.Size(400, 300);

            string[] ports = SerialPort.GetPortNames();

            // Dropdown for COM Port 1
            Label lblComPort1 = new Label() { Text = "COM Port 1:", Location = new System.Drawing.Point(20, 20), AutoSize = true };
            ComboBox cmbComPort1 = new ComboBox() { Name = "cmbComPort1", Location = new System.Drawing.Point(120, 20), Width = 150 };
            cmbComPort1.Items.AddRange(ports);
            // Dropdown for COM Port 2
            Label lblComPort2 = new Label() { Text = "COM Port 2:", Location = new System.Drawing.Point(20, 60), AutoSize = true };
            ComboBox cmbComPort2 = new ComboBox() { Name = "cmbComPort2", Location = new System.Drawing.Point(120, 60), Width = 150 };
            cmbComPort2.Items.AddRange(ports);
            // Virtual COM Port
            Label lblVirtualPort = new Label() { Text = "Virtual Port:", Location = new System.Drawing.Point(20, 100), AutoSize = true };
            ComboBox cmbComPort3 = new ComboBox() { Name = "cmbComPort3", Location = new System.Drawing.Point(120, 100), Width = 150 };
            cmbComPort3.Items.AddRange(ports);

            // Start Button
            Button btnStart = new Button() { Text = "Старт", Location = new System.Drawing.Point(50, 200), Width = 100 };
            btnStart.Click += BtnStart_Click;
            // Stop Button
            Button btnStop = new Button() { Text = "Стоп", Location = new System.Drawing.Point(200, 200), Width = 100 };
            btnStop.Click += BtnStop_Click;

            this.Controls.Add(lblComPort1);
            this.Controls.Add(cmbComPort1);
            this.Controls.Add(lblComPort2);
            this.Controls.Add(cmbComPort2);
            this.Controls.Add(lblVirtualPort);
            this.Controls.Add(cmbComPort3);
            this.Controls.Add(btnStart);
            this.Controls.Add(btnStop);
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            try
            {
                var portList = new List<string>
                {
                    ((ComboBox)this.Controls["cmbComPort1"]).Text,
                    ((ComboBox)this.Controls["cmbComPort2"]).Text
                };
                var virtualPort = ((ComboBox)this.Controls["cmbComPort3"]).Text;


                var fileName = "config.ini";
                if (File.Exists(fileName))
                {
                    portList = new List<string>();
                    using (var fileStream = File.OpenRead(fileName))
                    using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                    {
                        string line;
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            var firstParam = "";
                            var secondParam = "";
                            var data = line.Split(':');
                            if (data.Length > 1)
                            {
                                firstParam = data[0];
                                secondParam = data[1];
                            }
                            else { continue; }
                            switch (firstParam) {
                                case "VirtualComPort":
                                    virtualPort = data[1];
                                    break;
                                default:
                                    if (firstParam.Contains("RealComPort"))
                                        portList.Add(data[1]);
                                    continue;
                            }
                        }
                    }
                }


                if (portList.Any(x => string.IsNullOrWhiteSpace(x)) || string.IsNullOrWhiteSpace(virtualPort))
                {
                    MessageBox.Show("Please configure all ports before starting.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _serialPortManager = new SerialPortManager(portList, virtualPort);
                _serialPortManager.OpenAllPorts();
                MessageBox.Show("Успешно!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting ports: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            try
            {
                _serialPortManager?.CloseAllPorts();
                MessageBox.Show("Ports successfully stopped!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping ports: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _serialPortManager?.CloseAllPorts();
        }
    }
}
