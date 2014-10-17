using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectControlRobot.Application.Interface;
using Microsoft.Kinect;
using System.Threading;

namespace KinectControlRobot.Application.Service
{
    public class KinectService : IKinectService
    {
        private readonly KinectSensor _currentKinectSensor;

        public KinectSensor CurrentKinectSensor { get { return _currentKinectSensor; } }

        public KinectService(KinectSensor kinectSensor)
        {
            _currentKinectSensor = kinectSensor;
        }

        public void SetupKinectSensor(EventHandler<ColorImageFrameReadyEventArgs> colorImageFreamReadyEventHandler,
            EventHandler<SkeletonFrameReadyEventArgs> skeletonFrameReadyEventHandler)
        {
            // Setup kinect streams info
            CurrentKinectSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            CurrentKinectSensor.ColorFrameReady += colorImageFreamReadyEventHandler;

            CurrentKinectSensor.SkeletonStream.Enable();
            CurrentKinectSensor.SkeletonFrameReady += skeletonFrameReadyEventHandler;

            // setup kinect audiosource info
            CurrentKinectSensor.AudioSource.BeamAngleMode = BeamAngleMode.Adaptive;
            CurrentKinectSensor.AudioSource.NoiseSuppression = true;
            CurrentKinectSensor.AudioSource.EchoCancellationMode = EchoCancellationMode.CancellationOnly;
            CurrentKinectSensor.AudioSource.AutomaticGainControlEnabled = false;
            CurrentKinectSensor.AudioSource.EchoCancellationSpeakerIndex = 0;
        }

        public void StopKinectSensor()
        {
            CurrentKinectSensor.Stop();
        }
    }
}
