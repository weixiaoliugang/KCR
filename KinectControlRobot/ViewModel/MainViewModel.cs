using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using KinectControlRobot.Service;
using KinectControlRobot.Interface;
using System.Windows.Media;
using System.Collections.ObjectModel;
using Microsoft.Kinect;
using System.Windows;

namespace KinectControlRobot.ViewModel
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
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IKinectService kinectService)
        {
            _kinectService = kinectService;

            for(int i=0;i<20;i++)
            {
                SkeletonJoints.Add(new Joint());
                SkeletonLines.AddGeometry(new LineGeometry(new Point(), new Point()));
            }
        }

#region Binding Property
        /// <summary>
        /// The <see cref="StatusCaption" /> property's name.
        /// </summary>
        public const string StatusCaptionPropertyName = "StatusCaption";

        private string _statusCaption = "Status Caption";

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

        private string _statusDescription = "Status Description";

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

        private string _statusHelperString = "Status Helper String";

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

        private string _statusColor = "#FF01DF3A";

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
        /// The <see cref="ButtonColor" /> property's name.
        /// </summary>
        public const string ButtonColorPropertyName = "ButtonColor";

        private string _buttonColor = "#FF444444";

        /// <summary>
        /// Sets and gets the ButtonColor property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ButtonColor
        {
            get
            {
                return _buttonColor;
            }
            set
            {
                Set(() => ButtonColor, ref _buttonColor, value);
            }
        }

        /// <summary>
        /// The <see cref="ButtonString" /> property's name.
        /// </summary>
        public const string ButtonStringPropertyName = "ButtonString";

        private string _buttonString = "准备";

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
        /// The <see cref="ButtonPressedColor" /> property's name.
        /// </summary>
        public const string ButtonPressedColorPropertyName = "ButtonPressedColor";

        private string _buttonPressedColor = "#FFBDBDBD";

        /// <summary>
        /// Sets and gets the ButtonPressedColor property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ButtonPressedColor
        {
            get
            {
                return _buttonPressedColor;
            }
            set
            {
                Set(() => ButtonPressedColor, ref _buttonPressedColor, value);
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
        /// The <see cref="SkeletonLines" /> property's name.
        /// </summary>
        public const string SkeletonLinesPropertyName = "SkeletonLines";

        private PathGeometry _skeletonLines = new PathGeometry();

        /// <summary>
        /// Sets and gets the SkeletonLines property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public PathGeometry SkeletonLines
        {
            get
            {
                return _skeletonLines;
            }
            set
            {
                Set(() => SkeletonLines, ref _skeletonLines, value);
            }
        }

#endregion

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}