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
        private MCUState _lastState;


        /// <summary>
        /// Occurs when [mcu State changed]. 
        /// </summary>
        public event Action<MCUState> StateChanged;

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
            if (_serialPort.BytesToRead == 32)
            {
                var buffer = new byte[32];
                _serialPort.Read(buffer, 0, 32);
                var receivedFrame = new ReceivedFrame(buffer);

                switch (receivedFrame.Parse())
                {
                    case ReceivedFrameFlag.SystemNormal:
                        State = MCUState.SystemNormal; break;
                    case ReceivedFrameFlag.ShakingHand:
                        break;
                    case ReceivedFrameFlag.SystemAbnormal:
                        State = MCUState.SystemAbnormal; break;
                    case ReceivedFrameFlag.Working:
                        State = MCUState.Working; break;
                    case ReceivedFrameFlag._Broken_:
                        break;
                }

                var currMCUState = State;
                if (currMCUState != _lastState)
                {
                    Action<MCUState> handler = StateChanged;
                    if (handler != null)
                    {
                        handler(currMCUState);
                    }

                    _lastState = currMCUState;
                }
            }
        }

        /// <summary>
        /// Connects to mcu. 
        /// </summary>
        public void Connect()
        {
            _serialPort.Open();

            var requestingFrame = new FrameToSend(FrameToSendFlag.Requesting);
            WriteAllBytes(requestingFrame.ToBytes());
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