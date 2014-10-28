using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        KinectSensor CurrentKinectSensor { get; set; }

        public event EventHandler<ColorImageFrameReadyEventArgs> ColorImageFrameReady;

        public event EventHandler<DepthImageFrameReadyEventArgs> DepthImageFrameReady;

        public event EventHandler<SkeletonFrameReadyEventArgs> SkeletonFrameReady;

        public event EventHandler<AllFramesReadyEventArgs> AllFrameReady;

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
        /// Stops the kinect sensor.
        /// </summary>
        void StopKinectSensor();
    }
}
