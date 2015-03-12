using System;
using KinectControlRobot.Application.Model;

namespace KinectControlRobot.Application.Interface
{
    /// <summary>
    /// Interface shows the methods and properties that a MCUService should implement 
    /// </summary>
    public interface IMCUService
    {
        /// <summary>
        /// Gets or sets the current mcu. 
        /// </summary>
        /// <value> The current mcu. </value>
        IMCU MCU { get; }

        /// <summary>
        /// Initializes this instance. 
        /// </summary>
        void Initialize();

        /// <summary>
        /// Initializes the instance asynchronously. 
        /// </summary>
        void InitializeAsynchronous();

        /// <summary>
        /// Sends the frame.
        /// </summary>
        /// <param name="frameToSend">The frame to send.</param>
        void SendFrame(FrameToSend frameToSend);

        /// <summary>
        /// Closes this instance. 
        /// </summary>
        void Close();

        /// <summary>
        /// Resets the mcu. 
        /// </summary>
        void ResetMCU();

        /// <summary>
        /// Stops the mcu. 
        /// </summary>
        void DisConnentMCU();
    }
}