using GalaSoft.MvvmLight.Messaging;
using KinectControlRobot.Application.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectControlRobot.Application.Message
{
    /// <summary>
    /// The Kinect Service Ready Message that a messenger should send for the registers
    /// </summary>
    public class KinectServiceReadyMessage : MessageBase
    {
        /// <summary>
        /// Gets the kinect service.
        /// </summary>
        /// <value>
        /// The kinect service.
        /// </value>
        public IKinectService KinectService { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectServiceReadyMessage"/> class.
        /// </summary>
        /// <param name="kinectService">The kinect service.</param>
        public KinectServiceReadyMessage(IKinectService kinectService)
        {
            KinectService = kinectService;
        }
    }
}
