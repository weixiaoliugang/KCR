using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KinectControlRobot.Interface
{
    public interface IKinectService
    {
        KinectSensor GetKinectSensor();
        bool CloseKinectSensor();
    }
}
