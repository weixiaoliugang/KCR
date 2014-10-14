using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using KinectControlRobot.Application.Service;
using KinectControlRobot.Application.Interface;
using System.Windows.Media;
using System.Collections.ObjectModel;
using Microsoft.Kinect;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using System.Linq;
using System;
using Coding4Fun.Kinect.Wpf;

namespace KinectControlRobot.Application.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private IKinectService _kinectService;
        private Int32Rect _rect = new Int32Rect(0, 0, 640, 480);

        private IMCUService _mcuService;

#if DEBUG
#endif

        [PreferredConstructor]
        public MainViewModel(IKinectService kinectService, IMCUService mcuService)
        {
            Initialize();
        }

        private void Initialize()
        {
            Task.Factory.StartNew(() =>
            {
                Parallel.Invoke(() =>
                {
                    _kinectService = new KinectService(() =>
                    {
                        StatusDescription = "Kinect已连接，程序正在尝试连接下位机。。。";
                    });
                    _kinectService.SetupKinectSensor(new EventHandler<ColorImageFrameReadyEventArgs>(ColorImageFrameReadyEventHandler),
                        new EventHandler<SkeletonFrameReadyEventArgs>(SkeletonFrameReadyEventHandler));
                }, () =>
                {
                    _mcuService = new MCUService(() =>
                    {
                        StatusDescription = "下位机已连接，程序正在尝试连接Kinect。。。";
                    });
                });

                CameraShadowColor = "#FF66B034";
                StatusColor = "#FF66B034";
                StatusCaption = "准备就绪";
                StatusDescription = "可以开始工作了";
                ButtonString = "准备";
            });
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel() { }

        #region KinectEventHandlers

        private void ColorImageFrameReadyEventHandler(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                if (imageFrame != null)
                {
                    byte[] _pixelData = new byte[imageFrame.PixelDataLength];
                    imageFrame.CopyPixelDataTo(_pixelData);

                    Camera.WritePixels(_rect, _pixelData.ToArray(), Camera.BackBufferStride, 0);
                }
            }
        }

        private void SkeletonFrameReadyEventHandler(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    Skeleton[] _skeletonData = new Skeleton[_kinectService.CurrentKinectSensor.SkeletonStream.FrameSkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(_skeletonData);
                    Skeleton skeleton = (from ske in _skeletonData
                                         where ske.TrackingState == SkeletonTrackingState.Tracked
                                         select ske).FirstOrDefault();
                    if (skeleton != null)
                    {
                        SetSkeletonData(skeleton);
                    }
                }
            }
        }

        private void SetSkeletonData(Skeleton skeleton)
        {
            HeadPoints.Clear();
            LeftHandPoints.Clear();
            RightHandPoints.Clear();
            LeftFootPoints.Clear();
            RightFootPoints.Clear();

            Joint scaledJoint;

            foreach (Joint joint in skeleton.Joints)
            {
                scaledJoint = SkeletalExtensions.ScaleTo(joint, 640, 480);

                switch (joint.JointType)
                {
                    case JointType.Head:
                        SkeletonJoints[0] = scaledJoint;
                        HeadPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        break;

                    case JointType.ShoulderCenter:
                        SkeletonJoints[1] = scaledJoint;
                        HeadPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        LeftHandPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        RightHandPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        break;

                    case JointType.ShoulderLeft:
                        SkeletonJoints[2] = scaledJoint;
                        LeftHandPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        break;

                    case JointType.ShoulderRight:
                        SkeletonJoints[3] = scaledJoint;
                        RightHandPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        break;

                    case JointType.ElbowLeft:
                        SkeletonJoints[4] = scaledJoint;
                        LeftHandPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        break;

                    case JointType.ElbowRight:
                        SkeletonJoints[5] = scaledJoint;
                        RightHandPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        break;

                    case JointType.WristLeft:
                        SkeletonJoints[6] = scaledJoint;
                        LeftHandPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        break;

                    case JointType.WristRight:
                        SkeletonJoints[7] = scaledJoint;
                        RightHandPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        break;

                    case JointType.HandLeft:
                        SkeletonJoints[8] = scaledJoint;
                        LeftHandPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        break;

                    case JointType.HandRight:
                        SkeletonJoints[9] = scaledJoint;
                        RightHandPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        break;

                    case JointType.Spine:
                        SkeletonJoints[10] = scaledJoint;
                        HeadPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        break;

                    case JointType.HipCenter:
                        SkeletonJoints[11] = scaledJoint;
                        HeadPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        LeftFootPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        RightFootPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        break;

                    case JointType.HipLeft:
                        SkeletonJoints[12] = scaledJoint;
                        LeftFootPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        break;

                    case JointType.HipRight:
                        SkeletonJoints[13] = scaledJoint;
                        RightFootPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        break;

                    case JointType.KneeLeft:
                        SkeletonJoints[14] = scaledJoint;
                        LeftFootPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        break;

                    case JointType.KneeRight:
                        SkeletonJoints[15] = scaledJoint;
                        RightFootPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        break;

                    case JointType.AnkleLeft:
                        SkeletonJoints[16] = scaledJoint;
                        LeftFootPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        break;

                    case JointType.AnkleRight:
                        SkeletonJoints[17] = scaledJoint;
                        RightFootPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        break;

                    case JointType.FootLeft:
                        SkeletonJoints[18] = scaledJoint;
                        LeftFootPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        break;

                    case JointType.FootRight:
                        SkeletonJoints[19] = scaledJoint;
                        RightFootPoints.Add(new Point(scaledJoint.Position.X, scaledJoint.Position.Y));
                        break;
                }
            }
            RaisePropertyChanged(SkeletonJointsPropertyName);

            RaisePropertyChanged(HeadPointsPropertyName);
            RaisePropertyChanged(LeftHandPointsPropertyName);
            RaisePropertyChanged(RightHandPointsPropertyName);
            RaisePropertyChanged(LeftFootPointsPropertyName);
            RaisePropertyChanged(RightFootPointsPropertyName);
        }
        #endregion

        #region Binding Property
        /// <summary>
        /// The <see cref="StatusCaption" /> property's name.
        /// </summary>
        public const string StatusCaptionPropertyName = "StatusCaption";

        private string _statusCaption = "等待连接。。。";

        /// <summary>
        /// Sets and gets the StatusCaption property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string StatusCaption
        {
            get
            {
                return _statusCaption;
            }
            set
            {
                Set(() => StatusCaption, ref _statusCaption, value);
            }
        }

        /// <summary>
        /// The <see cref="StatusDescription" /> property's name.
        /// </summary>
        public const string StatusDescriptionPropertyName = "StatusDescription";

        private string _statusDescription = "程序正在尝试连接。。。";

        /// <summary>
        /// Sets and gets the StatusDescription property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string StatusDescription
        {
            get
            {
                return _statusDescription;
            }
            set
            {
                Set(() => StatusDescription, ref _statusDescription, value);
            }
        }

        /// <summary>
        /// The <see cref="StatusHelperString" /> property's name.
        /// </summary>
        public const string StatusHelperStringPropertyName = "StatusHelperString";

        private string _statusHelperString = string.Empty;

        /// <summary>
        /// Sets and gets the StatusHelperString property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string StatusHelperString
        {
            get
            {
                return _statusHelperString;
            }
            set
            {
                Set(() => StatusHelperString, ref _statusHelperString, value);
            }
        }

        /// <summary>
        /// The <see cref="Camera" /> property's name.
        /// </summary>
        public const string CameraPropertyName = "Camera";

        private WriteableBitmap _camera = new WriteableBitmap(640, 480, 96, 96, PixelFormats.Bgr32, null);

        /// <summary>
        /// Sets and gets the Camera property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public WriteableBitmap Camera
        {
            get
            {
                return _camera;
            }
            set
            {
                Set(() => Camera, ref _camera, value);
            }
        }

        /// <summary>
        /// The <see cref="CameraShadowColor" /> property's name.
        /// </summary>
        public const string CameraShadowColorPropertyName = "CameraShadowColor";

        private string _cameraShadowColor = "#FFD7DF01";

        /// <summary>
        /// Sets and gets the CameraShadowColor property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string CameraShadowColor
        {
            get
            {
                return _cameraShadowColor;
            }
            set
            {
                Set(() => CameraShadowColor, ref _cameraShadowColor, value);
            }
        }

        /// <summary>
        /// The <see cref="StatusColor" /> property's name.
        /// </summary>
        public const string StatusColorPropertyName = "StatusColor";

        private string _statusColor = "#FFD7DF01";

        /// <summary>
        /// Sets and gets the StatusColor property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string StatusColor
        {
            get
            {
                return _statusColor;
            }
            set
            {
                Set(() => StatusColor, ref _statusColor, value);
            }
        }

        /// <summary>
        /// The <see cref="ButtonString" /> property's name.
        /// </summary>
        public const string ButtonStringPropertyName = "ButtonString";

        private string _buttonString = string.Empty;

        /// <summary>
        /// Sets and gets the ButtonString property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ButtonString
        {
            get
            {
                return _buttonString;
            }
            set
            {
                Set(() => ButtonString, ref _buttonString, value);
            }
        }

        /// <summary>
        /// The <see cref="SkeletonJoints" /> property's name.
        /// </summary>
        public const string SkeletonJointsPropertyName = "SkeletonJoints";

        private ObservableCollection<Joint> _skeletonJoints = new ObservableCollection<Joint>();

        /// <summary>
        /// Sets and gets the SkeletonJoints property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Joint> SkeletonJoints
        {
            get
            {
                return _skeletonJoints;
            }
            set
            {
                Set(() => SkeletonJoints, ref _skeletonJoints, value);
            }
        }

        /// <summary>
        /// The <see cref="HeadPoints" /> property's name.
        /// </summary>
        public const string HeadPointsPropertyName = "HeadPoints";

        private PointCollection _headPoints = new PointCollection();

        /// <summary>
        /// Sets and gets the HeadPoints property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public PointCollection HeadPoints
        {
            get
            {
                return _headPoints;
            }
            set
            {
                Set(() => HeadPoints, ref _headPoints, value);
            }
        }

        /// <summary>
        /// The <see cref="LeftHandPoints" /> property's name.
        /// </summary>
        public const string LeftHandPointsPropertyName = "LeftHandPoints";

        private PointCollection _leftHandPoints = new PointCollection();

        /// <summary>
        /// Sets and gets the LeftHandPoints property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public PointCollection LeftHandPoints
        {
            get
            {
                return _leftHandPoints;
            }
            set
            {
                Set(() => LeftHandPoints, ref _leftHandPoints, value);
            }
        }

        /// <summary>
        /// The <see cref="RightHandPoints" /> property's name.
        /// </summary>
        public const string RightHandPointsPropertyName = "RightHandPoints";

        private PointCollection _rightHandPoints = new PointCollection();

        /// <summary>
        /// Sets and gets the RightHandPoints property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public PointCollection RightHandPoints
        {
            get
            {
                return _rightHandPoints;
            }
            set
            {
                Set(() => RightHandPoints, ref _rightHandPoints, value);
            }
        }

        /// <summary>
        /// The <see cref="LeftFootPoints" /> property's name.
        /// </summary>
        public const string LeftFootPointsPropertyName = "LeftFootPoints";

        private PointCollection _leftFootPoints = new PointCollection();

        /// <summary>
        /// Sets and gets the LeftFootPoints property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public PointCollection LeftFootPoints
        {
            get
            {
                return _leftFootPoints;
            }
            set
            {
                Set(() => LeftFootPoints, ref _leftFootPoints, value);
            }
        }

        /// <summary>
        /// The <see cref="RightFootPoints" /> property's name.
        /// </summary>
        public const string RightFootPointsPropertyName = "RightFootPoints";

        private PointCollection _rightFootPoints = new PointCollection();

        /// <summary>
        /// Sets and gets the RightFootPoints property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public PointCollection RightFootPoints
        {
            get
            {
                return _rightFootPoints;
            }
            set
            {
                Set(() => RightFootPoints, ref _rightFootPoints, value);
            }
        }

        #endregion

        public override void Cleanup()
        {
            // Clean up if needed
            _kinectService.StopKinectSensor();
            _mcuService.StopMCU();

            base.Cleanup();
        }

        public void BeginCheckMCUStatus(int interval)
        {
            throw new NotImplementedException();
        }
    }
}