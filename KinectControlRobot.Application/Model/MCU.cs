using KinectControlRobot.Application.Interface;
using System;
using System.IO.Ports;

namespace KinectControlRobot.Application.Model
{
    /// <summary>
    /// MCU class have the methods and properties to interact with the mcu 
    /// </summary>
    public class MCU : IMCU, IDisposable
    {
        private readonly SerialPort _serialPort;

        /// <summary>
        /// Gets the State. 
        /// </summary>
        /// <value> The MCUState. </value>
        public MCUState State { get; private set; }

        public MCU(string serialPortName)
        {
            _serialPort = new SerialPort(serialPortName) { BaudRate = 115200 };
            _serialPort.DataReceived += _onSerialPortDataReceived;
            _serialPort.ErrorReceived += _onSerialPortErrorReceived;
        }

        private void _onSerialPortErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            _serialPort.Close();
            State = MCUState.DisConnected;
        }

        private void _onSerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // TODO: assign State when received mcu reporting 
        }

        /// <summary>
        /// Connects to mcu. 
        /// </summary>
        public void Connect()
        {
            _serialPort.Open();
        }

        /// <summary>
        /// Disconnect from the mcu. 
        /// </summary>
        public void DisConnect()
        {
            _serialPort.Close();
        }

        /// <summary>
        /// Writes the specified bytes. 
        /// </summary>
        /// <param name="bytes"> The bytes. </param>
        public void WriteAllBytes(byte[] bytes)
        {
            _serialPort.Write(bytes, 0, bytes.Length);
        }

        public void Dispose()
        {
            _serialPort.Dispose();
        }
    }
}