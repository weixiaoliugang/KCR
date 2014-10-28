using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectControlRobot.Application.Interface
{
    /// <summary>
    /// Interface shows a MCU should implement
    /// </summary>
    public interface IMCU
    {
        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The MCUStatus.
        /// </value>
        MCUStatus Status { get; }

        /// <summary>
        /// Connects to mcu.
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnect from the mcu.
        /// </summary>
        void DisConnect();
    }

    /// <summary>
    /// The MCU Status
    /// </summary>
    public enum MCUStatus
    {
        DisConnected,
        SystemNormal,
        SystemAbnormal,
        Working,
    }
}
