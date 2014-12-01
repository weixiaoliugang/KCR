using GalaSoft.MvvmLight.Ioc;
using KinectControlRobot.Application.Interface;
using KinectControlRobot.Application.Model;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace KinectControlRobot.Application.Service
{
    /// <summary>
    /// Service has the methods and the properties to interact with the mcu 
    /// </summary>
    public class MCUService : IMCUService
    {
        private MCUState _lastState;

        /// <summary>
        /// Gets or sets the current mcu. 
        /// </summary>
        /// <value> The current mcu. </value>
        public IMCU CurrentMCU { get; private set; }

        private void _checkCanExecute()
        {
            if (CurrentMCU == null || CurrentMCU.State != MCUState.SystemNormal)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Occurs when [mcu State changed]. 
        /// </summary>
        public event Action<MCUState> MCUStateChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="MCUService" /> class. 
        /// </summary>
        [PreferredConstructor]
        public MCUService(string serialPortName = "COM3", IMCU mcu = null)
        {
            CurrentMCU = mcu ?? new MCU(serialPortName);
        }

        private void _startQueryMCUState()
        {
            _checkCanExecute();

            // call the lambda expression each tick 
            var checkStateTimer = new Timer(200);
            checkStateTimer.Elapsed += (o, e) =>
            {
                MCUState currMCUState = CurrentMCU.State;
                if (currMCUState != _lastState)
                {
                    Action<MCUState> handler = MCUStateChanged;
                    if (handler != null)
                    {
                        handler(currMCUState);
                    }

                    _lastState = currMCUState;
                }
            };
            checkStateTimer.Start();
        }

        /// <summary>
        /// Resets the mcu. 
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void ResetMCU()
        {
            _checkCanExecute();

            throw new NotImplementedException();
        }

        /// <summary>
        /// Stops the mcu. 
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void DisConnentMCU()
        {
            _checkCanExecute();

            CurrentMCU.DisConnect();
        }

        /// <summary>
        /// Initializes this instance. 
        /// </summary>
        public void Initialize()
        {
            CurrentMCU.Connect();

            do
            {
                System.Threading.Thread.Sleep(200);
            } while (CurrentMCU.State != MCUState.SystemNormal);

            _lastState = CurrentMCU.State;
            _startQueryMCUState();
        }

        /// <summary>
        /// Initializes the instance asynchronously. 
        /// </summary>
        public void InitializeAsynchronous()
        {
            Task.Factory.StartNew(Initialize);
        }

        /// <summary>
        /// Closes this instance. 
        /// </summary>
        public void Close()
        {
            CurrentMCU = null;
        }
    }
}