namespace KinectControlRobot.Application.Interface
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

    public enum FrameToSendFlag
    {
        Requesting = (byte)0x00,
        Performing = (byte)0x0f,
        Halting = (byte)0xf0,
    }

    public enum ReceivedFrameFlag
    {
        SystemNormal = (byte)0x01,
        SystemAbnormal = (byte)0x04,
        Working = (byte)0x02,
        ShakingHand = (byte)0x00,
        _Broken_ = (byte)0xff,
    }
}