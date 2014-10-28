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
    /// <summary>
    /// Class of the KinectService has the methods and properties to interact with the Kinect Sensor
    /// </summary>
    public class KinectService : IKinectService
    {
        private KinectSensor _currentKinectSensor;

        /// <summary>
        /// Gets or sets the current kinect sensor.
        /// </summary>
        /// <value>
        /// The current kinect sensor.
        /// </value>
        /// <exception cref="System.ArgumentException"></exception>
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

        private void _checkCanExecute()
        {
            if (_currentKinectSensor == null || _currentKinectSensor.Status != KinectStatus.Connected)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectService"/> class.
        /// </summary>
        [PreferredConstructor]
        public KinectService() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectService"/> class.
        /// </summary>
        /// <param name="kinectSensor">The kinect sensor.</param>
        public KinectService(KinectSensor kinectSensor)
        {
            CurrentKinectSensor = kinectSensor;
        }

        /// <summary>
        /// Occurs when [color image frame ready].
        /// </summary>
        public event EventHandler<ColorImageFrameReadyEventArgs> ColorImageFrameReady
        {
            add
            {
                _checkCanExecute();

                if (value != null)
                {
                    _currentKinectSensor.ColorFrameReady += value;
                }
            }
            remove
            {
                _checkCanExecute();

                if (value != null)
                {
                    _currentKinectSensor.ColorFrameReady -= value;
                }
            }
        }

        /// <summary>
        /// Occurs when [depth image frame ready].
        /// </summary>
        public event EventHandler<DepthImageFrameReadyEventArgs> DepthImageFrameReady
        {
            add
            {
                _checkCanExecute();

                if (value != null)
                {
                    _currentKinectSensor.DepthFrameReady += value;
                }
            }
            remove
            {
                _checkCanExecute();

                if (value != null)
                {
                    _currentKinectSensor.DepthFrameReady -= value;
                }
            }
        }

        /// <summary>
        /// Occurs when [skeleton frame ready].
        /// </summary>
        public event EventHandler<SkeletonFrameReadyEventArgs> SkeletonFrameReady
        {
            add
            {
                _checkCanExecute();

                if (value != null)
                {
                    _currentKinectSensor.SkeletonFrameReady += value;
                }
            }
            remove
            {
                _checkCanExecute();

                if (value != null)
                {
                    _currentKinectSensor.SkeletonFrameReady -= value;
                }
            }
        }

        /// <summary>
        /// Occurs when [all frame ready].
        /// </summary>
        public event EventHandler<AllFramesReadyEventArgs> AllFrameReady
        {
            add
            {
                _checkCanExecute();

                if (value != null)
                {
                    _currentKinectSensor.AllFramesReady += value;
                }
            }
            remove
            {
                _checkCanExecute();

                if (value != null)
                {
                    _currentKinectSensor.AllFramesReady -= value;
                }
            }
        }

        /// <summary>
        /// Setups the kinect sensor.
        /// </summary>
        /// <param name="colorImageFormat">The color image format.</param>
        /// <param name="depthImageFormat">The depth image format.</param>
        /// <param name="transformSmoothParameters">The transform smooth parameters.</param>
        public void SetupKinectSensor(ColorImageFormat colorImageFormat,
            DepthImageFormat depthImageFormat, TransformSmoothParameters transformSmoothParameters)
        {
            _checkCanExecute();

            // Setup kinect streams info
            _currentKinectSensor.ColorStream.Enable(colorImageFormat);
            _currentKinectSensor.DepthStream.Enable(depthImageFormat);
            _currentKinectSensor.SkeletonStream.Enable(transformSmoothParameters);
            //_currentKinectSensor.SkeletonStream.Enable(new TransformSmoothParameters
            //{
            //    Smoothing = 0.5f,
            //    Correction = 0.5f,
            //    Prediction = 0.5f,
            //    JitterRadius = 0.05f,
            //    MaxDeviationRadius = 0.04f
            //});

            // setup kinect audiosource info
            _currentKinectSensor.AudioSource.BeamAngleMode = BeamAngleMode.Adaptive;
            _currentKinectSensor.AudioSource.NoiseSuppression = true;
            _currentKinectSensor.AudioSource.EchoCancellationMode = EchoCancellationMode.CancellationOnly;
            _currentKinectSensor.AudioSource.AutomaticGainControlEnabled = false;
            _currentKinectSensor.AudioSource.EchoCancellationSpeakerIndex = 0;
        }

        /// <summary>
        /// Starts the kinect sensor.
        /// </summary>
        public void StartKinectSensor()
        {
            _checkCanExecute();

            _currentKinectSensor.Start();
        }

        /// <summary>
        /// Stops the kinect sensor.
        /// </summary>
        public void StopKinectSensor()
        {
            _checkCanExecute();

            _currentKinectSensor.Stop();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
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

        /// <summary>
        /// Initializes the instance asynchronously.
        /// </summary>
        public void InitializeAsynchronous()
        {
            Task.Factory.StartNew(() =>
                {
                    Initialize();
                });
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            _currentKinectSensor = null;
        }
    }
}
