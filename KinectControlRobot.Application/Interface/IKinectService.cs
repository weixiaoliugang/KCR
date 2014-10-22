using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KinectControlRobot.Application.Interface
{
    public interface IKinectService
    {
        KinectSensor CurrentKinectSensor { get; set; }

        void RegisterColorImageFrameReadyEvent(EventHandler<ColorImageFrameReadyEventArgs> handler);

        void RegisterDepthImageFrameReadyEvent(EventHandler<DepthImageFrameReadyEventArgs> handler);

        void RegisterSkeletonFrameReadyEvent(EventHandler<SkeletonFrameReadyEventArgs> handler);

        void RegisterAllFrameReadyEvent(EventHandler<AllFramesReadyEventArgs> handler);

        void SetupKinectSensor(ColorImageFormat colorImageFormat, DepthImageFormat depthImageFormat);

        void Initialize();

        void InitializeAsynchronous();

        void Close();

        void StartKinectSensor();

        void StopKinectSensor();
    }
}
