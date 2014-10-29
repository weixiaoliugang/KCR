using System;
using KinectControlRobot.Application.Interface;

namespace KinectControlRobot.Application.Model
{
    /// <summary>
    /// MCU class have the methods and properties to interact with the mcu
    /// </summary>
    public class MCU : IMCU
    {
        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The MCUStatus.
        /// </value>
        public MCUStatus Status
        {
            get { return _checkStatus(); }
        }

        /// <summary>
        /// Check the current mcu status.
        /// </summary>
        /// <returns>
        /// The current mcu status
        /// </returns>
        private MCUStatus _checkStatus()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Connects to mcu.
        /// </summary>
        public void Connect()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disconnect from the mcu.
        /// </summary>
        public void DisConnect()
        {
            throw new NotImplementedException();
        }
    }
}
