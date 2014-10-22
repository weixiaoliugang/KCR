using System.Windows;
using KinectControlRobot.Application.ViewModel;
using CustomChrome;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Kinect;
using Microsoft.Practices.ServiceLocation;
using KinectControlRobot.Application.Interface;
using Kinect.Toolbox;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using KinectControlRobot.Application.Message;
using GalaSoft.MvvmLight.Threading;

namespace KinectControlRobot.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : CustomChromeWindow
    {
        private SkeletonDisplayManager _skeletonDisplayManager;
        private Skeleton[] _skeletons;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
            : base(new Thickness(0), 24, new CornerRadius(6), new Thickness(0))
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();

            Messenger.Default.Register<KinectServiceReadyMessage>(this, (msg) =>
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    msg.KinectService.RegisterSkeletonFrameReadyEvent(SkeletonFrameReadyEventHandler);
                    _skeletonDisplayManager = new SkeletonDisplayManager(msg.KinectService.CurrentKinectSensor,
                        SkeletonCanvas);
                }));
        }

        private void SkeletonFrameReadyEventHandler(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (var skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame == null)
                    return;

                skeletonFrame.GetSkeletons(ref _skeletons);

                if (_skeletons.All(s => s.TrackingState == SkeletonTrackingState.NotTracked))
                    return;

                _skeletonDisplayManager.Draw(_skeletons, false);
            }
        }
    }
}