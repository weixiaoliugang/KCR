using System;
using System.IO;
using Microsoft.Kinect;

namespace KinectControlRobot.Application.Interface
{
    /// <summary>
    /// Interface shows the methods and properties a KinectService should implement
    /// </summary>
    public interface IKinectService
    {
        /// <summary>
        /// Gets or sets the current kinect sensor.
        /// </summary>
        /// <value>
        /// The current kinect sensor.
        /// </value>
        KinectSensor KinectSensor { get; }

        /// <summary>
        /// Gets the coordinate mapper.
        /// </summary>
        /// <value>
        /// The coordinate mapper.
        /// </value>
        CoordinateMapper CoordinateMapper { get; }

        /// <summary>
        /// Occurs when [color image frame ready].
        /// </summary>
        event EventHandler<ColorImageFrameReadyEventArgs> ColorImageFrameReady;

        /// <summary>
        /// Occurs when [depth image frame ready].
        /// </summary>
        event EventHandler<DepthImageFrameReadyEventArgs> DepthImageFrameReady;

        /// <summary>
        /// Occurs when [skeleton frame ready].
        /// </summary>
        event EventHandler<SkeletonFrameReadyEventArgs> SkeletonFrameReady;

        /// <summary>
        /// Occurs when [all frame ready].
        /// </summary>
        event EventHandler<AllFramesReadyEventArgs> AllFrameReady;

        /// <summary>
        /// Setups the kinect sensor.
        /// </summary>
        /// <param name="colorImageFormat">The color image format.</param>
        /// <param name="depthImageFormat">The depth image format.</param>
        /// <param name="transformSmoothParameter">The transform smooth parameter.</param>
        void SetupKinectSensor(ColorImageFormat colorImageFormat, 
            DepthImageFormat depthImageFormat, TransformSmoothParameters transformSmoothParameter);

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
        /// Starts the kinect sensor.
        /// </summary>
        void StartKinectSensor();

        /// <summary>
        /// Starts the audio stream.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns></returns>
        Stream StartAudioStream(TimeSpan timeSpan);

        /// <summary>
        /// Stops the kinect sensor.
        /// </summary>
        void StopKinectSensor();
    }
}
