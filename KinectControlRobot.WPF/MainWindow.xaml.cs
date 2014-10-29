using System.Windows;
using KinectControlRobot.Application.ViewModel;
using CustomChrome;
using Microsoft.Kinect;
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
    // ReSharper disable once RedundantExtendsListEntry
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

            // handle the KinectServiceReadyMessage and register the event using the method here
            // for the KinectSensor may take time to get and this ctor is called at the moment the 
            // app is started
            Messenger.Default.Register<KinectServiceReadyMessage>(this, msg =>
                        {
                            // this is running in a background thread so the event is handled in background
                            msg.KinectService.SkeletonFrameReady += SkeletonFrameReadyEventHandler;
                            _skeletonDisplayManager = new SkeletonDisplayManager(msg.KinectService.CurrentKinectSensor,
                                SkeletonCanvas);
                        });
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

                DispatcherHelper.CheckBeginInvokeOnUI(() => _skeletonDisplayManager.Draw(_skeletons, false));
            }
        }
    }
}