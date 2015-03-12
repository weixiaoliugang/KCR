using GalaSoft.MvvmLight.Ioc;
using KinectControlRobot.Application.Interface;
using Microsoft.Kinect;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KinectControlRobot.Application.Service
{
    /// <summary>
    /// Class of the KinectService has the methods and properties to interact with the Kinect Sensor 
    /// </summary>
    public class KinectService : IKinectService
    {
        /// <summary>
        /// Gets or sets the current kinect sensor. 
        /// </summary>
        /// <value> The current kinect sensor. </value>
        public KinectSensor KinectSensor { get; private set; }

        /// <summary>
        /// Gets the coordinate mapper. 
        /// </summary>
        /// <value> The coordinate mapper. </value>
        public CoordinateMapper CoordinateMapper { get; private set; }

        private void _checkCanExecute()
        {
            if (KinectSensor == null || KinectSensor.Status != KinectStatus.Connected)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KinectService" /> class. this ctor is for
        /// the ServiceLocator.Current.GetInstance() to use
        /// </summary>
        [PreferredConstructor]

        // ReSharper disable once UnusedMember.Global 
        public KinectService(KinectSensor kinectSensor = null)
        {
            KinectSensor = kinectSensor;
        }

        /// <summary>
        /// Setups the kinect sensor. 
        /// </summary>
        /// <param name="colorImageFormat">          The color image format. </param>
        /// <param name="depthImageFormat">          The depth image format. </param>
        /// <param name="transformSmoothParameters"> The transform smooth parameters. </param>
        public void SetupKinectSensor(ColorImageFormat colorImageFormat,
            DepthImageFormat depthImageFormat, TransformSmoothParameters transformSmoothParameters)
        {
            _checkCanExecute();

            // Setup kinect streams info 
            KinectSensor.ColorStream.Enable(colorImageFormat);
            KinectSensor.DepthStream.Enable(depthImageFormat);
            KinectSensor.SkeletonStream.Enable(transformSmoothParameters);

            // setup kinect audiosource info 
            KinectSensor.AudioSource.BeamAngleMode = BeamAngleMode.Adaptive;
            KinectSensor.AudioSource.NoiseSuppression = true;
            KinectSensor.AudioSource.EchoCancellationMode = EchoCancellationMode.CancellationOnly;
            KinectSensor.AudioSource.AutomaticGainControlEnabled = false;
            KinectSensor.AudioSource.EchoCancellationSpeakerIndex = 0;
        }

        /// <summary>
        /// Starts the kinect sensor. 
        /// </summary>
        public void StartKinectSensor()
        {
            _checkCanExecute();

            KinectSensor.Start();
        }

        /// <summary>
        /// Starts the audio stream. 
        /// </summary>
        /// <param name="timeSpan"> The time span. </param>
        /// <returns></returns>
        public Stream StartAudioStream(TimeSpan timeSpan)
        {
            _checkCanExecute();

            return KinectSensor.AudioSource.Start(timeSpan);
        }

        /// <summary>
        /// Stops the kinect sensor. 
        /// </summary>
        public void StopKinectSensor()
        {
            _checkCanExecute();

            KinectSensor.Stop();
        }

        /// <summary>
        /// Initializes this instance. 
        /// </summary>
        public void Initialize()
        {
            // while haven't got the sensor, keep fetching 
            while (KinectSensor == null)
            {
                System.Threading.Thread.Sleep(200);
                KinectSensor = (from sensor in KinectSensor.KinectSensors
                                where sensor.Status == KinectStatus.Connected
                                select sensor).FirstOrDefault();
            }

            CoordinateMapper = new CoordinateMapper(KinectSensor);
        }

        /// <summary>
        /// Initializes the instance asynchronously. 
        /// </summary>
        public void InitializeAsynchronous()
        {
            Task.Factory.StartNew(Initialize);
        }

        /// <summary>
        /// Closes this instance. 
        /// </summary>
        public void Close()
        {
            KinectSensor = null;
        }
    }
}