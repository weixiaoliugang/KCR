using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectControlRobot.Application.Interface;
using System.Timers;
using KinectControlRobot.Application.Model;
using Microsoft.Practices.ServiceLocation;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;

namespace KinectControlRobot.Application.Service
{
    /// <summary>
    /// Service has the methods and the properties to interact with the mcu
    /// </summary>
    public class MCUService : IMCUService
    {
        private IMCU _currentMCU;
        private MCUStatus _lastStatus;

        /// <summary>
        /// Gets or sets the current mcu.
        /// </summary>
        /// <value>
        /// The current mcu.
        /// </value>
        /// <exception cref="System.ArgumentException"></exception>
        public IMCU CurrentMCU
        {
            get { return _currentMCU; }
            set
            {
                if (value is IMCU)
                {
                    _currentMCU = value;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        private void _checkCanExecute()
        {
            if (_currentMCU == null || _currentMCU.Status != MCUStatus.SystemNormal)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Occurs when [mcu status changed].
        /// </summary>
        public event Action<MCUStatus> MCUStatusChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="MCUService"/> class.
        /// </summary>
        [PreferredConstructor]
        public MCUService() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MCUService"/> class.
        /// </summary>
        /// <param name="mcu">The mcu.</param>
        public MCUService(IMCU mcu)
        {
            CurrentMCU = mcu;
        }

        private void _startQueryMCUStatus()
        {
            _checkCanExecute();

            // call the lambda expression each tick
            Timer checkStatusTimer = new Timer(200);
            checkStatusTimer.Elapsed += new ElapsedEventHandler((o, e) =>
            {
                MCUStatus currMCUStatus = _currentMCU.Status;
                if (currMCUStatus != _lastStatus)
                {
                    Action<MCUStatus> handler = MCUStatusChanged;
                    if (handler != null)
                    {
                        handler(currMCUStatus);
                    }

                    _lastStatus = currMCUStatus;
                }
            });
            checkStatusTimer.Start();
        }

        /// <summary>
        /// Starts the mcu.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void StartMCU()
        {
            _checkCanExecute();

            throw new NotImplementedException();
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
        public void StopMCU()
        {
            _checkCanExecute();

            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            if (_currentMCU == null)
            {
                _currentMCU = ServiceLocator.Current.GetInstance<IMCU>();
                while (_currentMCU.Status != MCUStatus.SystemNormal)
                {
                    System.Threading.Thread.Sleep(200);
                    _currentMCU.Connect();
                }

                _lastStatus = _currentMCU.Status;
                _startQueryMCUStatus();
            }
        }

        /// <summary>
        /// Initializes the instance asynchronously.
        /// </summary>
        public void InitializeAsynchronous()
        {
            Task.Factory.StartNew(() =>
            {
                Initialize();
            });
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            _currentMCU = null;
        }
    }
}
