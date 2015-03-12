using System.Collections.Generic;
using System.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using KinectControlRobot.Application.Helper;
using KinectControlRobot.Application.Interface;
using KinectControlRobot.Application.Message;
using KinectControlRobot.Application.Model;
using Microsoft.Kinect;
using Microsoft.Practices.ServiceLocation;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KinectControlRobot.Application.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to. 
    /// <para> See http://www.galasoft.ch/mvvm </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        // fields for the kinect event handler 
        private IKinectService _kinectService;

        private readonly Int32Rect _rect = new Int32Rect(0, 0, 640, 480);
        private byte _frameCounter;

        // fields for the mcu event handler 
        private IMCUService _mcuService;
        private FrameToSend _haltingFrame = new FrameToSend(FrameToSendFlag.Halting);

        // flags indicate the app's atatus 
        private bool _isReady;

        private bool _isWorking;
        private bool _isPreparing;

#if DEBUG

        // this is a class that only unittest would use 
        public MainViewModel(IKinectService kinectService, IMCUService mcuService)
        {
            _kinectService = kinectService;
            _mcuService = mcuService;

            CameraShadowColor = "#FF66B034";
            StateColor = "#FF66B034";
            _setValuesOnSystemReady();

            _isReady = true;
        }

#endif

        [PreferredConstructor]

        // ReSharper disable once UnusedMember.Global 
        public MainViewModel()
        {
            // for the initialize progress may get the kinect sensor, make sure it'll get it until
            // it's really running
            if (!IsInDesignModeStatic)
            {
                _initialize();
            }
        }

        private readonly object _statusDescriptionLock = new object();

        private void _initialize()
        {
            // get the services in a background thread so it won't block the UI thread 
            Task.Factory.StartNew(() =>
            {
                // get the KinectService and the MCUService parallel this call is a block one 
                Parallel.Invoke(() =>
                {
                    _kinectService = ServiceLocator.Current.GetInstance<IKinectService>();

                    // this initialize might be blocked 
                    _kinectService.Initialize();
                    _kinectService.SetupKinectSensor(ColorImageFormat.RgbResolution640x480Fps30,
                        DepthImageFormat.Resolution640x480Fps30, new TransformSmoothParameters
                                                                {
                                                                    Smoothing = 0.5f,
                                                                    Correction = 0.5f,
                                                                    Prediction = 0.5f,
                                                                    JitterRadius = 0.05f,
                                                                    MaxDeviationRadius = 0.04f
                                                                });
                    _kinectService.KinectSensor.AllFramesReady += _onAllFrameReady;
                    _kinectService.StartKinectSensor();

                    // send the message to tell the whole app that the kinect is ready for the might
                    // wanna do things with the sensor
                    Messenger.Default.Send(new KinectServiceReadyMessage(_kinectService));

                    lock (_statusDescriptionLock)
                    {
                        StateDescription = "Kinect已连接，程序正在尝试连接下位机。。。";
                    }
                }, () =>
                {
                    _mcuService = ServiceLocator.Current.GetInstance<IMCUService>();

                    // this initialize might be blocked 
                    _mcuService.Initialize();
                    _mcuService.MCU.StateChanged += _onMCUStateChanged;

                    lock (_statusDescriptionLock)
                    {
                        StateDescription = "下位机已连接，程序正在尝试连接Kinect。。。";
                    }
                });

                CameraShadowColor = "#FF66B034";
                StateColor = "#FF66B034";
                _setValuesOnSystemReady();

                _isReady = true;
            });
        }

        private void _setValuesOnSystemReady()
        {
            StateCaption = "系统就绪";
            StateDescription = "可以开始准备了";
            ButtonString = "准备";
        }

        #region EventHandler

        private void _onAllFrameReady(object sender, AllFramesReadyEventArgs e)
        {
            Parallel.Invoke(() =>
            {
                using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
                {
                    if (imageFrame != null)
                    {
                        var pixelData = new byte[imageFrame.PixelDataLength];
                        imageFrame.CopyPixelDataTo(pixelData);

                        ViewImage.WritePixels(_rect, pixelData.ToArray(), ViewImage.BackBufferStride, 0);
                    }
                }
            }, () =>
            {
                if (_isWorking)
                {
                    // use the _frameCounter to make this code run every 1/10 second for this event is
                    // fired every 1/30 second
                    if ((++_frameCounter) == 3)
                    {
                        _frameCounter = 0;

                        Task.Factory.StartNew(() =>
                        {
                            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
                            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
                            {
                                var skeleton = skeletonFrame.GetSkeletons()
                                     .FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);

                                if (skeleton != null && depthFrame != null)
                                {
                                    var frameToSend = new FrameToSend(FrameToSendFlag.Performing);

                                    Parallel.Invoke(() =>
                                    {
                                        var leftBodyState = BodyStateDetector.GetAngleAndRotation(skeleton,
                                            BodySide.Left);
                                        var rightBodyState = BodyStateDetector.GetAngleAndRotation(skeleton,
                                            BodySide.Right);

                                        //TODO: Process data and save it to the list which is supposed to send
                                    }, () =>
                                    {
                                        var mappedHandLeft =
                                            _kinectService.CoordinateMapper.MapSkeletonPointToDepthPoint(
                                                skeleton.Joints[JointType.HandLeft].Position,
                                                _kinectService.KinectSensor.DepthStream.Format);
                                        var mappedHandRight =
                                            _kinectService.CoordinateMapper.MapSkeletonPointToDepthPoint(
                                                skeleton.Joints[JointType.HandRight].Position,
                                                _kinectService.KinectSensor.DepthStream.Format);

                                        var isHandLeftOpen = BodyStateDetector.IsHandOpen(depthFrame,
                                            mappedHandLeft);
                                        var isHandRightOpen = BodyStateDetector.IsHandOpen(depthFrame,
                                            mappedHandRight);

                                        //TODO: Process data and save it to the list which is supposed to send
                                    });

                                    _mcuService.SendFrame(frameToSend);
                                }
                            }
                        });
                    }
                }
                else if (_isPreparing)
                {
                    using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
                    {
                        var skeleton = skeletonFrame.GetSkeletons()
                             .FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);

                        if (skeleton != null)
                        {
                            var leftBodyState = BodyStateDetector.GetAngleAndRotation(skeleton,
                                BodySide.Left);
                            var rightBodyState = BodyStateDetector.GetAngleAndRotation(skeleton,
                                BodySide.Right);

                            if (PersonIsReady(leftBodyState,rightBodyState))
                            {
                                StateCaption = "正在工作";
                                StateDescription = "系统正常运行";

                                ButtonString = "停止";

                                _isWorking = true;
                            }
                        }
                    }
                }
                else
                {
                    // send halting frame to mcu to stop the movement
                    _mcuService.SendFrame(_haltingFrame);
                }
            });
        }

        private bool PersonIsReady(List<double> leftBodyState, List<double> rightBodyState)
        {
            //todo: TEST DATA
        }

        private void _onMCUStateChanged(MCUState mcuState)
        {
            switch (mcuState)
            {
                case MCUState.DisConnected:
                    _changeOnMCUError();

                    CameraShadowColor = "#FF3A3A3A";
                    StateCaption = "连接断开";
                    StateDescription = "程序正在尝试重新连接。。。";

                    Task.Factory.StartNew(() =>
                        {
                            while (_mcuService.MCU.State != MCUState.SystemNormal)
                            {
                                // connect mcu and reset robot 
                                System.Threading.Thread.Sleep(200);
                                _mcuService.ResetMCU();
                            }

                            CameraShadowColor = "#FF66B034";
                            StateColor = "#FF66B034";
                            StateHelperString = string.Empty;

                            _setValuesOnSystemReady();

                            _isReady = true;
                        });
                    break;

                case MCUState.SystemAbnormal:
                    _changeOnMCUError();

                    StateCaption = "系统异常";
                    StateDescription = "下位机出现异常";
                    break;

                case MCUState.Working:
                    if (!_isWorking)
                    {
                        _changeOnMCUError();

                        StateCaption = "系统异常";
                        StateDescription = "下位机报告仍在工作";
                    }
                    break;
            }
        }

        private void _changeOnMCUError()
        {
            StateColor = "#FF3A3A3A";
            StateHelperString = "请检查下位机故障";

            _isWorking = false;
            _isReady = false;
        }

        #endregion EventHandler

        #region Binding Property

        /// <summary>
        /// The <see cref="StateCaption" /> property's name. 
        /// </summary>
        public const string StateCaptionPropertyName = "StateCaption";

        private string _stateCaption = "等待连接。。。";

        /// <summary>
        /// Sets and gets the StateCaption property. Changes to that property's value raise the
        /// PropertyChanged event.
        /// </summary>
        public string StateCaption
        {
            get
            {
                return _stateCaption;
            }
            set
            {
                Set(() => StateCaption, ref _stateCaption, value);
            }
        }

        /// <summary>
        /// The <see cref="StateDescription" /> property's name. 
        /// </summary>
        public const string StateDescriptionPropertyName = "StateDescription";

        private string _stateDescription = "程序正在尝试连接。。。";

        /// <summary>
        /// Sets and gets the StateDescription property. Changes to that property's value raise the
        /// PropertyChanged event.
        /// </summary>
        public string StateDescription
        {
            get
            {
                return _stateDescription;
            }
            set
            {
                Set(() => StateDescription, ref _stateDescription, value);
            }
        }

        /// <summary>
        /// The <see cref="StateHelperString" /> property's name. 
        /// </summary>
        public const string StateHelperStringPropertyName = "StateHelperString";

        private string _stateHelperString = string.Empty;

        /// <summary>
        /// Sets and gets the StateHelperString property. Changes to that property's value raise the
        /// PropertyChanged event.
        /// </summary>
        public string StateHelperString
        {
            get
            {
                return _stateHelperString;
            }
            set
            {
                Set(() => StateHelperString, ref _stateHelperString, value);
            }
        }

        /// <summary>
        /// The <see cref="ViewImage" /> property's name. 
        /// </summary>
        public const string ViewImagePropertyName = "ViewImage";

        private WriteableBitmap _viewImage = new WriteableBitmap(640, 480, 96, 96, PixelFormats.Bgr32, null);

        /// <summary>
        /// Sets and gets the ViewImage property. Changes to that property's value raise the
        /// PropertyChanged event.
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
        /// Sets and gets the CameraShadowColor property. Changes to that property's value raise the
        /// PropertyChanged event.
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
        /// The <see cref="StateColor" /> property's name. 
        /// </summary>
        public const string StateColorPropertyName = "StateColor";

        private string _stateColor = "#FFD7DF01";

        /// <summary>
        /// Sets and gets the StateColor property. Changes to that property's value raise the
        /// PropertyChanged event.
        /// </summary>
        public string StateColor
        {
            get
            {
                return _stateColor;
            }
            set
            {
                Set(() => StateColor, ref _stateColor, value);
            }
        }

        /// <summary>
        /// The <see cref="ButtonString" /> property's name. 
        /// </summary>
        public const string ButtonStringPropertyName = "ButtonString";

        private string _buttonString = string.Empty;

        /// <summary>
        /// Sets and gets the ButtonString property. Changes to that property's value raise the
        /// PropertyChanged event.
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

        #endregion Binding Property

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
                            // pause the app for 500ms, so the mcu could reset all the motor to the right position
                            Thread.Sleep(500);

                            _isWorking = false;

                            _setValuesOnSystemReady();
                        }
                        else
                        {
                            StateCaption = "请准备";
                            StateDescription = "请保证双臂自然下垂";

                            ButtonString = "停止";

                            _isPreparing = true;
                        }
                    },
                    () => _isReady));
            }
        }

        #endregion RelayCommand

        public override void Cleanup()
        {
            // Clean up if needed 
            _kinectService.StopKinectSensor();
            _kinectService.Close();

            _mcuService.DisConnentMCU();
            _mcuService.Close();

            base.Cleanup();
        }
    }
}