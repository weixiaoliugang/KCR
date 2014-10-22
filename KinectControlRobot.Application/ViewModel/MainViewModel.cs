using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using KinectControlRobot.Application.Service;
using KinectControlRobot.Application.Interface;
using System.Windows.Media;
using System.Collections.ObjectModel;
using Microsoft.Kinect;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using System.Timers;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using System.Linq;
using System;
using Coding4Fun.Kinect.Wpf;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using System.Collections.Generic;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using KinectControlRobot.Application.Message;

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
        private byte[] _pixelData;
        private Skeleton[] _skeletonData;
        private readonly Int32Rect _rect = new Int32Rect(0, 0, 640, 480);

        private IMCUService _mcuService;

        private bool _isReady = false;
        private bool _isWorking = false;

#if DEBUG
        public MainViewModel(IKinectService kinectService, IMCUService mcuService)
        {
            _kinectService = kinectService;
            _mcuService = mcuService;

            CameraShadowColor = "#FF66B034";
            StatusColor = "#FF66B034";
            StatusCaption = "系统就绪";
            StatusDescription = "可以开始准备了";
            ButtonString = "准备";

            _isReady = true;
        }
#endif

        [PreferredConstructor]
        public MainViewModel()
        {
            _Initialize();
        }

        private void _Initialize()
        {
            Messenger.Default.Register<KinectServiceReadyMessage>(this, (msg) =>
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        _kinectService.RegisterColorImageFrameReadyEvent(_ColorImageFrameReadyEventHandler);
                        _kinectService.StartKinectSensor();
                    });
                });

            Task.Factory.StartNew(() =>
            {
                Parallel.Invoke(() =>
                {
                    _kinectService = ServiceLocator.Current.GetInstance<IKinectService>();
                    _kinectService.Initialize();
                    _kinectService.SetupKinectSensor(ColorImageFormat.RgbResolution640x480Fps30,
                        DepthImageFormat.Resolution640x480Fps30);

                    Messenger.Default.Send(new KinectServiceReadyMessage(_kinectService));

                    StatusDescription = "Kinect已连接，程序正在尝试连接下位机。。。";
                }, () =>
                {
                    //_mcuService = ServiceLocator.Current.GetInstance<IMCUService>();
                    //_mcuService.Initialize();
                    //_mcuService.MCUStatusChanged += MCUStatusChangedEventHandler;

                    //StatusDescription = "下位机已连接，程序正在尝试连接Kinect。。。";
                });

                CameraShadowColor = "#FF66B034";
                StatusColor = "#FF66B034";
                StatusCaption = "系统就绪";
                StatusDescription = "可以开始准备了";
                ButtonString = "准备";

                _isReady = true;
            });
        }

        #region EventHandler

        private void _MCUStatusChangedEventHandler(MCUStatus mcuStatus)
        {
            switch (mcuStatus)
            {
                case MCUStatus.DisConnected:
                    _ChangeOnMCUError();

                    CameraShadowColor = "#FF3A3A3A";
                    StatusCaption = "连接断开";
                    StatusDescription = "程序正在尝试重新连接。。。";

                    // TODO: try to connect MCU and then set _isReady to true in background thread
                    Task.Factory.StartNew(() =>
                        {
                            while (_mcuService.CurrentMCU.Status != MCUStatus.SystemNormal)
                            {
                                // connect mcu and reset robot
                                System.Threading.Thread.Sleep(200);
                                _mcuService.ResetMCU();
                            }

                            CameraShadowColor = "#FF66B034";
                            StatusColor = "#FF66B034";
                            StatusCaption = "系统就绪";
                            StatusDescription = "可以开始准备了";
                            StatusHelperString = string.Empty;

                            ButtonString = "准备";

                            _isReady = true;
                        });
                    break;
                case MCUStatus.SystemAbnormal:
                    _ChangeOnMCUError();

                    StatusCaption = "系统异常";
                    StatusDescription = "下位机出现异常";
                    break;
                case MCUStatus.Working:
                    if (!_isWorking)
                    {
                        _ChangeOnMCUError();

                        StatusCaption = "系统异常";
                        StatusDescription = "下位机报告仍在工作";
                    }
                    break;
            }
        }

        private void _ChangeOnMCUError()
        {
            StatusColor = "#FF3A3A3A";
            StatusHelperString = "请检查下位机故障";

            _isWorking = false;
            _isReady = false;
        }

        private void _ColorImageFrameReadyEventHandler(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                if (imageFrame != null)
                {
                    _pixelData = new byte[imageFrame.PixelDataLength];
                    imageFrame.CopyPixelDataTo(_pixelData);

                    ViewImage.WritePixels(_rect, _pixelData.ToArray(), ViewImage.BackBufferStride, 0);
                }
            }
        }

        private void _SkeletonFrameReadyEventHandler(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    _skeletonData = new Skeleton[_kinectService.CurrentKinectSensor.SkeletonStream.FrameSkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(_skeletonData);
                    Skeleton skeleton = (from ske in _skeletonData
                                         where ske.TrackingState == SkeletonTrackingState.Tracked
                                         select ske).FirstOrDefault();
                    if (skeleton != null)
                    {

                    }
                }
            }
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
        /// The <see cref="ViewImage" /> property's name.
        /// </summary>
        public const string ViewImagePropertyName = "ViewImage";

        private WriteableBitmap _viewImage = new WriteableBitmap(640, 480, 96, 96, PixelFormats.Bgr32, null);

        /// <summary>
        /// Sets and gets the ViewImage property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public WriteableBitmap ViewImage
        {
            get
            {
                return _viewImage;
            }
            set
            {
                Set(() => ViewImage, ref _viewImage, value);
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

        #endregion

        #region RelayCommand

        private RelayCommand _workCommand;

        /// <summary>
        /// Gets the WorkCommand.
        /// </summary>
        public RelayCommand WorkCommand
        {
            get
            {
                return _workCommand
                    ?? (_workCommand = new RelayCommand(
                    () =>
                    {
                        if (!WorkCommand.CanExecute(null))
                        {
                            return;
                        }

                        if (_isWorking)
                        {
                            StatusCaption = "系统就绪";
                            StatusDescription = "可以开始准备了";

                            ButtonString = "准备";
                            _isWorking = false;

                            // TODO: stop work here

                        }
                        else
                        {
                            StatusCaption = "正在工作";
                            StatusDescription = "系统正常运行";

                            ButtonString = "停止";
                            _isWorking = true;

                            // TODO: start work here

                        }
                    },
                    () => _isReady));
            }
        }
        #endregion

        public override void Cleanup()
        {
            // Clean up if needed
            _kinectService.StopKinectSensor();
            _kinectService.Close();

            //_mcuService.StopMCU();
            //_mcuService.Close();

            base.Cleanup();
        }
    }
}