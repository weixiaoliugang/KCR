﻿using System;
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
                            msg.KinectService.SkeletonFrameReady += _onSkeletonFrameReady;
                            _skeletonDisplayManager = new SkeletonDisplayManager(msg.KinectService.KinectSensor,
                                SkeletonCanvas);
                        });
        }

        private void _onSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (var skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame == null)
                    return;

                var skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                skeletonFrame.GetSkeletons(ref skeletons);

                if (skeletons.All(s => s.TrackingState == SkeletonTrackingState.NotTracked))
                    return;

                DispatcherHelper.CheckBeginInvokeOnUI(() => _skeletonDisplayManager.Draw(skeletons, false));
            }
        }
    }
}