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
        /// <summary>
        /// Gets or sets the current mcu. 
        /// </summary>
        /// <value> The current mcu. </value>
        public IMCU MCU { get; private set; }

        private void _checkCanExecute()
        {
            if (MCU == null)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MCUService" /> class. 
        /// </summary>
        [PreferredConstructor]
        public MCUService(string serialPortName = "COM3", IMCU mcu = null)
        {
            MCU = mcu ?? new MCU(serialPortName);
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

            MCU.DisConnect();
        }

        /// <summary>
        /// Initializes this instance. 
        /// </summary>
        public void Initialize()
        {
            MCU.Connect();

            do
            {
                System.Threading.Thread.Sleep(500);
            } while (MCU.State != MCUState.SystemNormal);
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
            MCU = null;
        }
    }
}