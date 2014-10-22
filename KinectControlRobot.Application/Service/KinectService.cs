using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectControlRobot.Application.Interface;
using Microsoft.Kinect;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;

namespace KinectControlRobot.Application.Service
{
    public class KinectService : IKinectService
    {
        private KinectSensor _currentKinectSensor;

        public KinectSensor CurrentKinectSensor
        {
            get { return _currentKinectSensor; }
            set
            {
                if (value is KinectSensor)
                {
                    _currentKinectSensor = value;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        private void _CheckCanExecute()
        {
            if (_currentKinectSensor == null || _currentKinectSensor.Status != KinectStatus.Connected)
            {
                throw new InvalidOperationException();
            }
        }

        [PreferredConstructor]
        public KinectService() { }

        public KinectService(KinectSensor kinectSensor)
        {
            CurrentKinectSensor = kinectSensor;
        }

        public void RegisterColorImageFrameReadyEvent(EventHandler<ColorImageFrameReadyEventArgs> handler)
        {
            _CheckCanExecute();

            _currentKinectSensor.ColorFrameReady += handler;
        }

        public void RegisterDepthImageFrameReadyEvent(EventHandler<DepthImageFrameReadyEventArgs> handler)
        {
            _CheckCanExecute();

            _currentKinectSensor.DepthFrameReady += handler;
        }

        public void RegisterSkeletonFrameReadyEvent(EventHandler<SkeletonFrameReadyEventArgs> handler)
        {
            _CheckCanExecute();

            _currentKinectSensor.SkeletonFrameReady += handler;
        }

        public void RegisterAllFrameReadyEvent(EventHandler<AllFramesReadyEventArgs> handler)
        {
            _CheckCanExecute();

            _currentKinectSensor.AllFramesReady += handler;
        }

        public void SetupKinectSensor(ColorImageFormat colorImageFormat, DepthImageFormat depthImageFormat)
        {
            _CheckCanExecute();

            // Setup kinect streams info
            _currentKinectSensor.ColorStream.Enable(colorImageFormat);
            _currentKinectSensor.DepthStream.Enable(depthImageFormat);
            _currentKinectSensor.SkeletonStream.Enable(new TransformSmoothParameters
            {
                Smoothing = 0.5f,
                Correction = 0.5f,
                Prediction = 0.5f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f
            });

            // setup kinect audiosource info
            _currentKinectSensor.AudioSource.BeamAngleMode = BeamAngleMode.Adaptive;
            _currentKinectSensor.AudioSource.NoiseSuppression = true;
            _currentKinectSensor.AudioSource.EchoCancellationMode = EchoCancellationMode.CancellationOnly;
            _currentKinectSensor.AudioSource.AutomaticGainControlEnabled = false;
            _currentKinectSensor.AudioSource.EchoCancellationSpeakerIndex = 0;
        }

        public void StartKinectSensor()
        {
            _CheckCanExecute();

            _currentKinectSensor.Start();
        }

        public void StopKinectSensor()
        {
            _CheckCanExecute();

            _currentKinectSensor.Stop();
        }

        public void Initialize()
        {
            // while haven't got the sensor, keep fetching
            while (_currentKinectSensor == null)
            {
                System.Threading.Thread.Sleep(200);
                _currentKinectSensor = (from sensor in KinectSensor.KinectSensors
                                        where sensor.Status == KinectStatus.Connected
                                        select sensor).FirstOrDefault();
            }
        }

        public void InitializeAsynchronous()
        {
            Task.Factory.StartNew(() =>
                {
                    Initialize();
                });
        }

        public void Close()
        {
            _currentKinectSensor = null;
        }
    }
}
