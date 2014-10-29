using System;

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
        /// <value>
        /// The current mcu.
        /// </value>
        IMCU CurrentMCU { get; }

        event Action<MCUStatus> MCUStatusChanged;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Initializes the instance asynchronously.
        /// </summary>
        void InitializeAsynchronous();

        /// <summary>
        /// Closes this instance.
        /// </summary>
        void Close();

        /// <summary>
        /// Starts the mcu.
        /// </summary>
        void StartMCU();

        /// <summary>
        /// Resets the mcu.
        /// </summary>
        void ResetMCU();

        /// <summary>
        /// Stops the mcu.
        /// </summary>
        void StopMCU();
    }
}
