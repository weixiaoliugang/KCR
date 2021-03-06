﻿namespace KinectControlRobot.Application.Interface
{
    /// <summary>
    /// Interface shows a MCU should implement 
    /// </summary>
    public interface IMCU
    {
        /// <summary>
        /// Gets the State. 
        /// </summary>
        /// <value> The MCUState. </value>
        MCUState State { get; }

        /// <summary>
        /// Connects to mcu. 
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnect from the mcu. 
        /// </summary>
        void DisConnect();

        /// <summary>
        /// Writes the specified bytes. 
        /// </summary>
        /// <param name="bytes"> The bytes. </param>
        void WriteAllBytes(byte[] bytes);
    }

    /// <summary>
    /// The MCU State 
    /// </summary>
    public enum MCUState
    {
        DisConnected,
        SystemNormal,
        SystemAbnormal,
        Working,
    }
}