using GalaSoft.MvvmLight.Messaging;
using KinectControlRobot.Application.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectControlRobot.Application.Message
{
    public class KinectServiceReadyMessage:MessageBase
    {
        public IKinectService KinectService { get; private set; }

        public KinectServiceReadyMessage(IKinectService kinectService)
        {
            KinectService = kinectService;
        }
    }
}
