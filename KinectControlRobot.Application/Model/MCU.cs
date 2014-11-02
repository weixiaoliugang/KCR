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
        /// Gets the State.
        /// </summary>
        /// <value>
        /// The MCUState.
        /// </value>
        public MCUState State
        {
            get { return _checkState(); }
        }

        private MCUState _checkState()
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
