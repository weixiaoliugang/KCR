using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectControlRobot.Application.Interface;
using System.Timers;

namespace KinectControlRobot.Application.Model
{
    public class MCU : IMCU
    {
        public MCUStatus Status
        {
            get { return _CheckStatus(); }
        }

        public MCU()
        {
        }

        private MCUStatus _CheckStatus()
        {
            throw new NotImplementedException();
        }

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public void DisConnect()
        {
            throw new NotImplementedException();
        }
    }
}
