using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KinectControlRobot.Application.Interface
{
    public interface IKinectService
    {
        KinectSensor CurrentKinectSensor { get; }

        void SetupKinectSensor(EventHandler<ColorImageFrameReadyEventArgs> colorImageFreamReadyEventHandler,
            EventHandler<SkeletonFrameReadyEventArgs> skeletonFrameReadyEventHandler);

        void StopKinectSensor();
    }
}
