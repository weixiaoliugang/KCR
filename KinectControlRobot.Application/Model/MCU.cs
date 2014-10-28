using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectControlRobot.Application.Interface;
using System.Timers;

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
        /// Initializes a new instance of the <see cref="MCU"/> class.
        /// </summary>
        public MCU()
        {
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
